using System;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sonosthesia.Mesh
{
    [RequireComponent(typeof(MeshFilter))]
    public abstract class MeshController : MonoBehaviour
    {
        private UnityEngine.Mesh m_Mesh;
        
        [Flags]
        private enum GizmoMode
        {
            Vertices = 1 << 1, 
            Normals = 1 << 2
        }

        [SerializeField] private GizmoMode _gizmoMode;

        [SerializeField] private bool _rebuildOnUpdate;

        [SerializeField] private float _rebuildFrequency;
        
        [SerializeField] bool _recalculateNormals;
         
        private bool _rebuildRequested;
        private float _nextScheduledRebuild;
        private Vector3[] _vertices, _normals;
        
        protected virtual string MeshName => "ProceduralMesh";
        
        public UnityEngine.Mesh Mesh
        {
            get
            {
                if (m_Mesh != null)
                {
                    return m_Mesh;
                }

                m_Mesh = new UnityEngine.Mesh { name = MeshName };
                GetComponent<MeshFilter>().mesh = m_Mesh;
                return m_Mesh;
            }
        }

        protected void RequestRebuild()
        {
            _rebuildRequested = true;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }
#endif
        }

        protected void Rebuild()
        {
            _vertices = null;
            _normals = null;
            
            Mesh.Clear();

            UnityEngine.Mesh.MeshDataArray meshDataArray = UnityEngine.Mesh.AllocateWritableMeshData(1);
            UnityEngine.Mesh.MeshData data = meshDataArray[0];

            PopulateMeshData(data);

            UnityEngine.Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, Mesh);

            Mesh.RecalculateBounds();

            if (_recalculateNormals)
            {
                Mesh.RecalculateNormals();
            }

            // Debug.Log($"{this} mesh now has {Mesh.vertexCount} vertices and {Mesh.triangles.Length} indices");
            
            _nextScheduledRebuild = Time.time + 1f / _rebuildFrequency;
        }

        protected abstract void PopulateMeshData(UnityEngine.Mesh.MeshData data);
        
        protected virtual void Update()
        {
            if (_rebuildOnUpdate || (_rebuildRequested && (Time.time >= _nextScheduledRebuild || !Application.isPlaying)))
            {
                Rebuild();
            }
        }

        protected virtual void OnValidate()
        {
            Rebuild();
        }
        
        protected virtual void OnEnable()
        {
            _rebuildRequested = true;
        }

        protected virtual void OnDrawGizmos ()
        {
            UnityEngine.Mesh mesh = Mesh;
            
            if (mesh == null || _gizmoMode == 0) 
            {
                return;
            }
            
            Transform t = transform;
            
            bool drawVertices = (_gizmoMode & GizmoMode.Vertices) != 0;
            bool drawNormals = (_gizmoMode & GizmoMode.Normals) != 0 && mesh.HasVertexAttribute(VertexAttribute.Normal);
            
            _vertices ??= mesh.vertices;
            if (drawNormals && _normals == null) 
            {
                _normals = mesh.normals;
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