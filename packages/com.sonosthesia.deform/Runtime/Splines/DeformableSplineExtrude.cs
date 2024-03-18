using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Mesh;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

namespace Sonosthesia.Deform
{
    // pretty much copy pasted from com.unity.splines SplineExtrude.cs with obsolete methods removed and hooks added
    
    /// <summary>
    /// A component for creating a tube mesh from a Spline at runtime.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public abstract class DeformableSplineExtrude : MonoBehaviour
    {
        [SerializeField]
        bool m_Parallel;
        
        [SerializeField, Tooltip("The Spline to extrude.")]
        SplineContainer m_Container;

        [SerializeField, Tooltip("Enable to regenerate the extruded mesh when the target Spline is modified. Disable " +
             "this option if the Spline will not be modified at runtime.")]
        bool m_RebuildOnSplineChange;

        [SerializeField]
        bool m_RebuildOnUpdate;
        
        [SerializeField, Tooltip("The maximum number of times per-second that the mesh will be rebuilt.")]
        int m_RebuildFrequency = 30;

        [SerializeField, Tooltip("Automatically update any Mesh, Box, or Sphere collider components when the mesh is extruded.")]
#pragma warning disable 414
        bool m_UpdateColliders = true;
#pragma warning restore 414

        [SerializeField, Tooltip("The number of sides that comprise the radius of the mesh.")]
        int m_Sides = 8;

        [SerializeField, Tooltip("The number of edge loops that comprise the length of one unit of the mesh. The " +
             "total number of sections is equal to \"Spline.GetLength() * segmentsPerUnit\".")]
        float m_SegmentsPerUnit = 4;

        [SerializeField, Tooltip("Indicates if the start and end of the mesh are filled. When the Spline is closed this setting is ignored.")]
        bool m_Capped = true;

        [SerializeField, Tooltip("The radius of the extruded mesh.")]
        float m_Radius = .25f;

        [SerializeField, Tooltip("The section of the Spline to extrude.")]
        Vector2 m_Range = new Vector2(0f, 1f);
        
        [SerializeField]
        bool m_BypassDeformation = true;

        [SerializeField] 
        bool m_RecalculateNormals;
        
        // Note : for some reason recalculating tangents seems to crash Unity editor
        
        [SerializeField] 
        bool m_RecalculateTangents;
        
        UnityEngine.Mesh m_Mesh;
        bool m_RebuildRequested;
        float m_NextScheduledRebuild;
        
        // used for gizmos
        
        [Flags]
        private enum GizmoMode
        {
            Vertices = 1 << 1, 
            Normals = 1 << 2
        }

        [SerializeField] private GizmoMode _gizmoMode;
        
        private Vector3[] _vertices, _normals;

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

        /// <summary>How many sides make up the radius of the mesh.</summary>
        public int Sides
        {
            get => m_Sides;
            set => m_Sides = Mathf.Max(value, 3);
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
        public float Radius
        {
            get => m_Radius;
            set => m_Radius = Mathf.Max(value, .00001f);
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
            get => m_Container.Spline;
        }

        /// <summary>The Splines to extrude.</summary>
        public IReadOnlyList<Spline> Splines
        {
            get => m_Container.Splines;
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

            m_RebuildRequested = true;
        }

        protected virtual void Awake()
        {
            m_Mesh = new UnityEngine.Mesh {name = "SplineExtrude"};
            GetComponent<MeshFilter>().mesh = m_Mesh;
        }

        protected virtual void Start()
        {
            if (m_Container == null || m_Container.Spline == null)
            {
                Debug.LogError("Spline Extrude does not have a valid SplineContainer set.");
                return;
            }

            if((m_Mesh = GetComponent<MeshFilter>().sharedMesh) == null)
                Debug.LogError("SplineExtrude.createMeshInstance is disabled, but there is no valid mesh assigned. " +
                    "Please create or assign a writable mesh asset.");
        }

        protected virtual void OnEnable()
        {
            m_RebuildRequested = true;
            Spline.Changed += OnSplineChanged;
        }

        protected void OnDisable()
        {
            Spline.Changed -= OnSplineChanged;
        }

        void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
        {
            if (m_Container != null && Splines.Contains(spline) && m_RebuildOnSplineChange)
                m_RebuildRequested = true;
        }

        protected virtual void Update()
        {
            if (m_RebuildOnUpdate || (m_RebuildRequested && Time.time >= m_NextScheduledRebuild))
            {
                m_RebuildRequested = false;
                Rebuild();
                m_NextScheduledRebuild = Time.time + 1f / m_RebuildFrequency;
            }
        }

        /// <summary>
        /// Triggers the rebuild of a Spline's extrusion mesh and collider.
        /// </summary>
        private void Rebuild()
        {
            _vertices = null;
            _normals = null;

            m_Mesh.Clear();
            UnityEngine.Mesh.MeshDataArray meshDataArray = UnityEngine.Mesh.AllocateWritableMeshData(1);
            UnityEngine.Mesh.MeshData data = meshDataArray[0];
            
            Extrude(Splines[0], data, m_Radius, m_Sides, m_SegmentsPerUnit, m_Capped, m_Range);

            if (!m_BypassDeformation)
            {
                Deform(Splines[0], data, m_Radius, m_Sides, m_SegmentsPerUnit, m_Capped, m_Range);   
            }

            UnityEngine.Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, m_Mesh);
            
            m_Mesh.RecalculateBounds();
            
            if (m_RecalculateNormals)
            {
                m_Mesh.RecalculateNormals();
            }

            if (m_RecalculateTangents)
            {
                m_Mesh.RecalculateTangents();
            }

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
        
        private void Extrude<TSpline>(TSpline spline, UnityEngine.Mesh.MeshData data, float radius, int sides, float segmentsPerUnit, bool capped, float2 range) 
            where TSpline : ISpline
        {
            data.subMeshCount = 1;

            var span = Mathf.Abs(range.y - range.x);

            var segments = Mathf.Max((int)Mathf.Ceil(spline.GetLength() * span * segmentsPerUnit), 1);
            
            SplineMesh.GetVertexAndIndexCount(sides, segments, capped, spline.Closed, (float2)range, out int vertexCount, out int indexCount);
            
            var indexFormat = vertexCount >= ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16;
            
            data.SetIndexBufferParams(indexCount, indexFormat);
            data.SetVertexBufferParams(vertexCount, k_PipeVertexAttribs);

            NativeArray<SplineVertexData> vertices = data.GetVertexData<SplineVertexData>();
            if (indexFormat == IndexFormat.UInt16)
            {
                NativeArray<UInt16> indices = data.GetIndexData<UInt16>();
                ParallelSplineMesh.Extrude(m_Parallel, spline, vertices, indices, radius, sides, segments, capped, range);
            }
            else
            {
                NativeArray<UInt32> indices = data.GetIndexData<UInt32>();
                ParallelSplineMesh.Extrude(m_Parallel, spline, vertices, indices, radius, sides, segments, capped, range);
            }
            
            data.SetSubMesh(0, new SubMeshDescriptor(0, vertexCount));
        }

        protected abstract void Deform(ISpline spline, UnityEngine.Mesh.MeshData data, 
            float radius, int sides, float segmentsPerUnit, bool capped, float2 range);

        protected virtual void OnValidate()
        {
            m_RebuildRequested = true;
        }

        protected void OnDrawGizmos () 
        {
            if (m_Mesh == null || _gizmoMode == 0) 
            {
                return;
            }
            Transform t = transform;
            
            bool drawVertices = (_gizmoMode & GizmoMode.Vertices) != 0;
            bool drawNormals = (_gizmoMode & GizmoMode.Normals) != 0 && m_Mesh.HasVertexAttribute(VertexAttribute.Normal);
            
            _vertices ??= m_Mesh.vertices;
            if (drawNormals && _normals == null) 
            {
                _normals = m_Mesh.normals;
            }
            
            for (int i = 0; i < _vertices.Length; i++) 
            {
                Vector3 position = t.TransformPoint(_vertices[i]);
                if (drawVertices)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(position, 0.02f);
                }
                if (drawNormals)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(position, t.TransformDirection(_normals[i] * 0.2f));
                }
            }
        }
    }
}