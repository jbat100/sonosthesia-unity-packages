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
        [Flags]
        private enum GizmoMode
        {
            Vertices = 1 << 1, 
            Normals = 1 << 2
        }
        
        [Flags]
        private enum MeshOptimizationMode 
        {
            ReorderIndices = 1 << 0, 
            ReorderVertices = 1 << 1
        }
        
        [SerializeField] private GizmoMode _gizmoMode;

        [SerializeField] private bool _rebuildOnUpdate;

        [SerializeField] private float _rebuildFrequency;
        
        [SerializeField] private bool _recalculateBounds;
        
        [SerializeField] private bool _recalculateNormals;
        
        [SerializeField] private bool _recalculateTangents;

        [SerializeField] private MeshOptimizationMode _meshOptimization = 0;
        
        private UnityEngine.Mesh _mesh;
        private bool _rebuildRequested;
        private float _nextScheduledRebuild;
        private Vector3[] _vertices, _normals;
        
        protected virtual string MeshName => "ProceduralMesh";
        
        public UnityEngine.Mesh Mesh
        {
            get
            {
                if (_mesh != null)
                {
                    return _mesh;
                }
                _mesh = new UnityEngine.Mesh { name = MeshName };
                return _mesh;
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

            if (_recalculateBounds)
            {
                Mesh.RecalculateBounds();
            }
            if (_recalculateNormals)
            {
                Mesh.RecalculateNormals();
            }
            if (_recalculateTangents)
            {
                _mesh.RecalculateTangents();                
            }

            switch (_meshOptimization)
            {
                case MeshOptimizationMode.ReorderIndices:
                    _mesh.OptimizeIndexBuffers();
                    break;
                case MeshOptimizationMode.ReorderVertices:
                    _mesh.OptimizeReorderVertexBuffer();
                    break;
                default:
                {
                    if (_meshOptimization != 0) 
                    {
                        _mesh.Optimize();
                    }
                    break;
                }
            }

            // Debug.Log($"{this} mesh now has {Mesh.vertexCount} vertices and {Mesh.triangles.Length} indices");
            
            _nextScheduledRebuild = Time.time + 1f / _rebuildFrequency;
        }

        protected abstract void PopulateMeshData(UnityEngine.Mesh.MeshData data);
        
        protected virtual void Awake()
        {
            GetComponent<MeshFilter>().mesh = Mesh;
        }
        
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