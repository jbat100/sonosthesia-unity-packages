using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Sonosthesia.Builder
{
    // pretty much copy pasted from com.unity.splines SplineExtrude.cs with obsolete methods removed and hooks added
    
    /// <summary>
    /// A component for creating a tube mesh from a Spline at runtime.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class DeformableSplineExtrude : MonoBehaviour
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
        
        Mesh m_Mesh;
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

        protected void Reset()
        {
            TryGetComponent(out m_Container);

            if (TryGetComponent<MeshFilter>(out var filter))
                filter.sharedMesh = m_Mesh = CreateMeshAsset();

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

        protected void Start()
        {
            if (m_Container == null || m_Container.Spline == null)
            {
                Debug.LogError("Spline Extrude does not have a valid SplineContainer set.");
                return;
            }

            if((m_Mesh = GetComponent<MeshFilter>().sharedMesh) == null)
                Debug.LogError("SplineExtrude.createMeshInstance is disabled, but there is no valid mesh assigned. " +
                    "Please create or assign a writable mesh asset.");

            Rebuild();
        }

        protected void OnEnable()
        {
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

        protected void Update()
        {
            if (m_RebuildRequested && Time.time >= m_NextScheduledRebuild)
            {
                Rebuild();
                Process(m_Mesh);
            }
        }

        /// <summary>
        /// Triggers the rebuild of a Spline's extrusion mesh and collider.
        /// </summary>
        public void Rebuild()
        {
            if((m_Mesh = GetComponent<MeshFilter>().sharedMesh) == null)
                return;
            
            SplineMesh.Extrude(Splines, m_Mesh, m_Radius, m_Sides, m_SegmentsPerUnit, m_Capped, m_Range);
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

        protected void OnValidate()
        {
            Rebuild();
        }

        internal Mesh CreateMeshAsset()
        {
            var mesh = new Mesh();
            mesh.name = name;

#if UNITY_EDITOR
            var scene = SceneManager.GetActiveScene();
            var sceneDataDir = "Assets";

            if (!string.IsNullOrEmpty(scene.path))
            {
                var dir = Path.GetDirectoryName(scene.path);
                sceneDataDir = $"{dir}/{Path.GetFileNameWithoutExtension(scene.path)}";
                if (!Directory.Exists(sceneDataDir))
                    Directory.CreateDirectory(sceneDataDir);
            }

            var path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath($"{sceneDataDir}/SplineExtrude_{mesh.name}.asset");
            UnityEditor.AssetDatabase.CreateAsset(mesh, path);
            mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>(path);
            UnityEditor.EditorGUIUtility.PingObject(mesh);
#endif
            return mesh;
        }

        protected virtual void Process(Mesh mesh)
        {
            
        }
        
        // code adapted from com.unity.splines SplineMesh.cs
        
        const float k_RadiusMin = .00001f, k_RadiusMax = 10000f;
        const int k_SidesMin = 3, k_SidesMax = 2084;
        const int k_SegmentsMin = 2, k_SegmentsMax = 4096;
        
        static readonly VertexAttributeDescriptor[] k_PipeVertexAttribs = new VertexAttributeDescriptor[]
        {
            new (VertexAttribute.Position),
            new (VertexAttribute.Normal),
            new (VertexAttribute.TexCoord0, dimension: 2)
        };
        
        // The logic around when caps and closing is a little complicated and easy to confuse. This wraps settings in a
        // consistent way so that methods aren't working with mixed data.
        struct Settings
        {
            public int sides { get; private set; }
            public int segments { get; private set; }
            public bool capped { get; private set; }
            public bool closed { get; private set; }
            public float2 range { get; private set; }
            public float radius { get; private set; }

            public Settings(int sides, int segments, bool capped, bool closed, float2 range, float radius)
            {
                this.sides = math.clamp(sides, k_SidesMin, k_SidesMax);
                this.segments = math.clamp(segments, k_SegmentsMin, k_SegmentsMax);
                this.range = new float2(math.min(range.x, range.y), math.max(range.x, range.y));
                this.closed = math.abs(1f - (this.range.y - this.range.x)) < float.Epsilon && closed;
                this.capped = capped && !this.closed;
                this.radius = math.clamp(radius, k_RadiusMin, k_RadiusMax);
            }
        }

        static void GetVertexAndIndexCount(Settings settings, out int vertexCount, out int indexCount)
        {
            vertexCount = settings.sides * (settings.segments + (settings.capped ? 2 : 0));
            indexCount = settings.sides * 6 * (settings.segments - (settings.closed ? 0 : 1)) + (settings.capped ? (settings.sides - 2) * 3 * 2 : 0);
        }
        
        public static void Extrude<TSplineType, TVertexType, TIndexType>(
            TSplineType spline,
            NativeArray<TVertexType> vertices,
            NativeArray<TIndexType> indices,
            float radius,
            int sides,
            int segments,
            bool capped,
            float2 range)
            where TSplineType : ISpline
            where TVertexType : struct, SplineMesh.ISplineVertexData
            where TIndexType : struct
        {
            Extrude(spline, vertices, indices, new Settings(sides, segments, capped, spline.Closed, range, radius));
        }
        
        public static void Extrude<TSplineType, TVertexType, TIndexType>(
            TSplineType spline,
            NativeArray<TVertexType> vertices,
            NativeArray<TIndexType> indices,
            float radius,
            int sides,
            int segments,
            bool capped,
            float2 range)
            where TSplineType : ISpline
            where TVertexType : struct, SplineMesh.ISplineVertexData
            where TIndexType : struct
        {
            Extrude(spline, vertices, indices, new Settings(sides, segments, capped, spline.Closed, range, radius));
        }
        
        /// <summary>
        /// Extrude a mesh along a list of splines in a tube-like shape.
        /// </summary>
        /// <param name="splines">The splines to extrude.</param>
        /// <param name="mesh">A mesh that will be cleared and filled with vertex data for the shape.</param>
        /// <param name="radius">The radius of the extruded mesh.</param>
        /// <param name="sides">How many sides make up the radius of the mesh.</param>
        /// <param name="segmentsPerUnit">The number of edge loops that comprise the length of one unit of the mesh.</param>
        /// <param name="capped">Whether the start and end of the mesh is filled. This setting is ignored when spline is closed.</param>
        /// <param name="range">
        /// The section of the Spline to extrude. This value expects a normalized interpolation start and end.
        /// I.e., [0,1] is the entire Spline, whereas [.5, 1] is the last half of the Spline.
        /// </param>
        /// <typeparam name="T">A type implementing ISpline.</typeparam>
        public static void Extrude<T>(IReadOnlyList<T> splines, Mesh mesh, float radius, int sides, float segmentsPerUnit, bool capped, float2 range) where T : ISpline
        {
            mesh.Clear();
            var meshDataArray = Mesh.AllocateWritableMeshData(1);
            var data = meshDataArray[0];
            data.subMeshCount = 1;

            var totalVertexCount = 0;
            var totalIndexCount = 0;
            var settings = new Settings[splines.Count];
            var span = Mathf.Abs(range.y - range.x);
            var splineMeshOffsets = new (int indexStart, int vertexStart)[splines.Count];
            for (int i = 0; i < splines.Count; ++i)
            {
                var spline = splines[i];
                
                var segments = Mathf.Max((int)Mathf.Ceil(spline.GetLength() * span * segmentsPerUnit), 1);
                settings[i] = new Settings(sides, segments, capped, spline.Closed, range, radius);
            
                GetVertexAndIndexCount(settings[i], out var vertexCount, out var indexCount);

                splineMeshOffsets[i] = (totalIndexCount, totalVertexCount);
                totalVertexCount += vertexCount;
                totalIndexCount += indexCount;
            }
            
            var indexFormat = totalVertexCount >= ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16;
            
            data.SetIndexBufferParams(totalIndexCount, indexFormat);
            data.SetVertexBufferParams(totalVertexCount, k_PipeVertexAttribs);

            var vertices = data.GetVertexData<SplineVertexData>();
            if (indexFormat == IndexFormat.UInt16)
            {
                var indices = data.GetIndexData<UInt16>();
                for (int i = 0; i < splines.Count; ++i)
                    SplineMesh.Extrude(splines[i], vertices, indices, settings[i], splineMeshOffsets[i].vertexStart, splineMeshOffsets[i].indexStart);
            }
            else
            {
                var indices = data.GetIndexData<UInt32>();
                for (int i = 0; i < splines.Count; ++i)
                    SplineMesh.Extrude(splines[i], vertices, indices, settings[i], splineMeshOffsets[i].vertexStart, splineMeshOffsets[i].indexStart);
            }
            
            data.SetSubMesh(0, new SubMeshDescriptor(0, totalIndexCount));

            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
            mesh.RecalculateBounds();
        }
    }
}