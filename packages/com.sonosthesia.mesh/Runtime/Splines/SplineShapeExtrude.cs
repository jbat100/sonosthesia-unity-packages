using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Sonosthesia.Mesh
{
/// <summary>
    /// A component for creating a tube mesh from a Spline at runtime.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class SplineShapeExtrude : MonoBehaviour
    {
        [SerializeField, Tooltip("The Spline to extrude.")]
        SplineContainer m_Container;

        [SerializeField, Tooltip("Enable to regenerate the extruded mesh when the target Spline is modified. Disable " +
             "this option if the Spline will not be modified at runtime.")]
        bool m_RebuildOnSplineChange;

        [SerializeField, Tooltip("The maximum number of times per-second that the mesh will be rebuilt.")]
        int m_RebuildFrequency = 30;

        [SerializeField, Tooltip("Automatically update any Mesh, Box, or Sphere collider components when the mesh is extruded.")]
#pragma warning disable 414
        bool m_UpdateColliders = true;
#pragma warning restore 414

        [SerializeField, Tooltip("The shape to be extruded.")]
        ExtrusionShape m_Shape;

        [SerializeField, Tooltip("The number of edge loops that comprise the length of one unit of the mesh. The " +
             "total number of sections is equal to \"Spline.GetLength() * segmentsPerUnit\".")]
        float m_SegmentsPerUnit = 4;

        [SerializeField, Tooltip("Indicates if the start and end of the mesh are filled. When the Spline is closed this setting is ignored.")]
        bool m_Capped = true;

        [SerializeField, Tooltip("The radius of the extruded mesh.")]
        float m_Scale = .25f;

        [SerializeField, Tooltip("The section of the Spline to extrude.")]
        Vector2 m_Range = new Vector2(0f, 1f);
        
        [SerializeField, Tooltip("Execute extrusion in parallel.")]
        bool m_Parallel;
        
        [SerializeField] 
        bool m_RecalculateNormals;

        UnityEngine.Mesh m_Mesh;
        bool m_RebuildRequested;
        float m_NextScheduledRebuild;
        
        /// <summary>The SplineContainer of the <see cref="Spline"/> to extrude.</summary>
        public SplineContainer Container
        {
            get => m_Container;
            set => m_Container = value;
        }

        /// <summary>
        /// Enable to regenerate the extruded mesh when the target Spline is modified. Disable this option if the Spline
        /// will not be modified at runtime.
        /// </summary>
        public bool RebuildOnSplineChange
        {
            get => m_RebuildOnSplineChange;
            set => m_RebuildOnSplineChange = value;
        }
        
        /// <summary>The maximum number of times per-second that the mesh will be rebuilt.</summary>
        public int RebuildFrequency
        {
            get => m_RebuildFrequency;
            set => m_RebuildFrequency = Mathf.Max(value, 1);
        }

        /// <summary>How many edge loops comprise the one unit length of the mesh.</summary>
        public float SegmentsPerUnit
        {
            get => m_SegmentsPerUnit;
            set => m_SegmentsPerUnit = Mathf.Max(value, .0001f);
        }

        /// <summary>Whether the start and end of the mesh is filled. This setting is ignored when spline is closed.</summary>
        public bool Capped
        {
            get => m_Capped;
            set => m_Capped = value;
        }

        /// <summary>The radius of the extruded mesh.</summary>
        public float Scale
        {
            get => m_Scale;
            set => m_Scale = Mathf.Max(value, .00001f);
        }

        /// <summary>
        /// The section of the Spline to extrude.
        /// </summary>
        public Vector2 Range
        {
            get => m_Range;
            set => m_Range = new Vector2(Mathf.Min(value.x, value.y), Mathf.Max(value.x, value.y));
        }
        
        /// <summary>The main Spline to extrude.</summary>
        public Spline Spline
        {
            get => m_Container?.Spline;
        }

        /// <summary>The Splines to extrude.</summary>
        public IReadOnlyList<Spline> Splines
        {
            get => m_Container?.Splines;
        }

        protected virtual void Reset()
        {
            TryGetComponent(out m_Container);

            if (TryGetComponent<MeshRenderer>(out var renderer) && renderer.sharedMaterial == null)
            {
                // todo Make Material.GetDefaultMaterial() public
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var mat = cube.GetComponent<MeshRenderer>().sharedMaterial;
                DestroyImmediate(cube);
                renderer.sharedMaterial = mat;
            }

            Rebuild();
        }

        private UnityEngine.Mesh Mesh
        {
            get
            {
                if (m_Mesh != null)
                {
                    return m_Mesh;
                }
                m_Mesh = new UnityEngine.Mesh {name = "SplineShapeExtrude"};
                GetComponent<MeshFilter>().mesh = m_Mesh;
                return m_Mesh;
            }
        }

        protected virtual void Start()
        {
            if (m_Container == null || m_Container.Spline == null)
            {
                Debug.LogError("Spline Extrude does not have a valid SplineContainer set.");
                return;
            }
        }

        protected virtual void OnEnable()
        {
            m_RebuildRequested = true;
            Spline.Changed += OnSplineChanged;
        }

        protected virtual void OnDisable()
        {
            Spline.Changed -= OnSplineChanged;
        }

        protected virtual void Update()
        {
            if(m_RebuildRequested && Time.time >= m_NextScheduledRebuild)
                Rebuild();
        }
        
        protected virtual void OnValidate()
        {
            Rebuild();
        }
        
        void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
        {
            if (m_Container != null && Splines.Contains(spline) && m_RebuildOnSplineChange)
                m_RebuildRequested = true;
        }
        

        /// <summary>
        /// Triggers the rebuild of a Spline's extrusion mesh and collider.
        /// </summary>
        public void Rebuild()
        {
            if (!m_Shape)
            {
                return; 
            }
            
            Mesh.Clear();
            UnityEngine.Mesh.MeshDataArray meshDataArray = UnityEngine.Mesh.AllocateWritableMeshData(1);
            UnityEngine.Mesh.MeshData data = meshDataArray[0];

            Spline spline = Splines[0];
            float span = Mathf.Abs(m_Range.y - m_Range.x);
            int segments = Mathf.Max((int)Mathf.Ceil(spline.GetLength() * span * m_SegmentsPerUnit), 1);

            ExtrusionSettings extrusionSettings = new ExtrusionSettings(segments, m_Capped, spline.Closed, m_Range);
            SplineShapeExtrusion.ShapeSettings shapeSettings = new SplineShapeExtrusion.ShapeSettings(m_Scale, m_Shape.PointCount, m_Shape.Closed);

            Extrude(spline, data, m_Shape, extrusionSettings, shapeSettings);

            UnityEngine.Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, Mesh);
            
            Mesh.RecalculateBounds();
            
            if (m_RecalculateNormals)
            {
                Mesh.RecalculateNormals();
            }
            
            m_NextScheduledRebuild = Time.time + 1f / m_RebuildFrequency;

#if UNITY_PHYSICS_MODULE
            if (m_UpdateColliders)
            {
                if (TryGetComponent<MeshCollider>(out var meshCollider))
                    meshCollider.sharedMesh = m_Mesh;

                if (TryGetComponent<BoxCollider>(out var boxCollider))
                {
                    boxCollider.center = m_Mesh.bounds.center;
                    boxCollider.size = m_Mesh.bounds.size;
                }

                if (TryGetComponent<SphereCollider>(out var sphereCollider))
                {
                    sphereCollider.center = m_Mesh.bounds.center;
                    var ext = m_Mesh.bounds.extents;
                    sphereCollider.radius = Mathf.Max(ext.x, ext.y, ext.z);
                }
            }
#endif
        }

        static readonly VertexAttributeDescriptor[] k_PipeVertexAttribs = new VertexAttributeDescriptor[]
        {
            new (VertexAttribute.Position),
            new (VertexAttribute.Normal),
            new (VertexAttribute.TexCoord0, dimension: 2)
        };
        
        private void Extrude<TSpline>(TSpline spline, UnityEngine.Mesh.MeshData data, ExtrusionShape shape, ExtrusionSettings extrusionSettings, SplineShapeExtrusion.ShapeSettings shapeSettings) 
            where TSpline : ISpline
        {
            data.subMeshCount = 1;
            
            NativeArray<ExtrusionShapePoint> points = shape.NativePoints;
            
            SplineShapeExtrusion.GetVertexAndIndexCount(extrusionSettings, shapeSettings, out int vertexCount, out int indexCount);
            
            var indexFormat = vertexCount >= ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16;
            
            data.SetIndexBufferParams(indexCount, indexFormat);
            data.SetVertexBufferParams(vertexCount, k_PipeVertexAttribs);

            NativeArray<SplineVertexData> vertices = data.GetVertexData<SplineVertexData>();
            if (indexFormat == IndexFormat.UInt16)
            {
                NativeArray<UInt16> indices = data.GetIndexData<UInt16>();
                SplineShapeExtrusion.Extrude(m_Parallel, spline, vertices, indices, points, extrusionSettings, shapeSettings);
            }
            else
            {
                NativeArray<UInt32> indices = data.GetIndexData<UInt32>();
                SplineShapeExtrusion.Extrude(m_Parallel, spline, vertices, indices, points, extrusionSettings, shapeSettings);
            }
            
            data.SetSubMesh(0, new SubMeshDescriptor(0, vertexCount));
        }
    }
}