using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

namespace Sonosthesia.Mesh
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public abstract class SplineCustomExtrude : MonoBehaviour
    {
        [SerializeField, Tooltip("The Spline to extrude.")]
        SplineContainer m_Container;

        [SerializeField, Tooltip("Enable to regenerate the extruded mesh when the target Spline is modified. Disable " +
                                 "this option if the Spline will not be modified at runtime.")]
        bool m_RebuildOnSplineChange;

        [SerializeField, Tooltip("The maximum number of times per-second that the mesh will be rebuilt.")]
        int m_RebuildFrequency = 30;

        [SerializeField, Tooltip("Rebuild on update no matter what.")]
        bool m_RebuildOnUpdate;

        [SerializeField,
         Tooltip("Automatically update any Mesh, Box, or Sphere collider components when the mesh is extruded.")]
#pragma warning disable 414
        bool m_UpdateColliders = true;
#pragma warning restore 414

        [SerializeField, Tooltip("The number of edge loops that comprise the length of one unit of the mesh. The " +
                                 "total number of sections is equal to \"Spline.GetLength() * segmentsPerUnit\".")]
        float m_SegmentsPerUnit = 4;

        [SerializeField,
         Tooltip(
             "Indicates if the start and end of the mesh are filled. When the Spline is closed this setting is ignored.")]
        bool m_Capped = true;

        [SerializeField, Tooltip("The section of the Spline to extrude.")]
        Vector2 m_Range = new Vector2(0f, 1f);

        [SerializeField, Tooltip("Execute extrusion in parallel.")]
        bool m_Parallel;

        [SerializeField] bool m_RecalculateNormals;
        
        [Flags]
        private enum GizmoMode
        {
            Vertices = 1 << 1, 
            Normals = 1 << 2
        }

        [SerializeField] private GizmoMode _gizmoMode;
        
        private Vector3[] _vertices, _normals;
        private UnityEngine.Mesh m_Mesh;
        private bool m_RebuildRequested;
        private float m_NextScheduledRebuild;


        /// <summary>The main Spline to extrude.</summary>
        public Spline Spline => m_Container?.Spline;

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

                m_Mesh = new UnityEngine.Mesh { name = "SplineExtrude" };
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
            if (m_RebuildOnUpdate || (m_RebuildRequested && Time.time >= m_NextScheduledRebuild))
                Rebuild();
        }

        protected virtual void OnValidate()
        {
            Rebuild();
        }

        void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
        {
            if (m_Container != null && Spline == spline && m_RebuildOnSplineChange)
                m_RebuildRequested = true;
        }


        protected abstract void PopulateMeshData(UnityEngine.Mesh.MeshData data, Spline spline, ExtrusionSettings extrusionSettings, bool parallel);
        
        /// <summary>
        /// Triggers the rebuild of a Spline's extrusion mesh and collider.
        /// </summary>
        public void Rebuild()
        {
            _vertices = null;
            _normals = null;
            
            Mesh.Clear();
            
            Spline spline = Spline;
            if (spline == null)
            {
                return;
            }
            
            UnityEngine.Mesh.MeshDataArray meshDataArray = UnityEngine.Mesh.AllocateWritableMeshData(1);
            UnityEngine.Mesh.MeshData data = meshDataArray[0];

            float splineLength = spline.GetLength();
            float span = Mathf.Abs(m_Range.y - m_Range.x);
            int segments = Mathf.Max((int)Mathf.Ceil(splineLength * span * m_SegmentsPerUnit), 1);
            
            // Debug.Log($"{this} rebuilding spline with length : {splineLength}, range {m_Range}, span {span}, segments {segments}");

            ExtrusionSettings extrusionSettings = new ExtrusionSettings(segments, m_Capped, spline.Closed, m_Range);

            PopulateMeshData(data, spline, extrusionSettings, m_Parallel);

            UnityEngine.Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, Mesh);

            Mesh.RecalculateBounds();

            if (m_RecalculateNormals)
            {
                Mesh.RecalculateNormals();
            }

            // Debug.Log($"{this} mesh now has {Mesh.vertexCount} vertices and {Mesh.triangles.Length} indices");
            
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