using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Sonosthesia.Mesh
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class JobProceduralMesh : MonoBehaviour
    {
        [System.Flags]
        public enum MeshOptimizationMode 
        {
            ReorderIndices = 1 << 0, 
            ReorderVertices = 1 << 1
        }

        [SerializeField] private MeshOptimizationMode _meshOptimization;
        
        public enum MaterialMode
        {
            Flat, 
            Ripple,
            LatLonMap,
            CubeMap
        }

        [SerializeField] private MaterialMode _materialMode;
        
        [SerializeField] private Material[] _materials;
        
        [System.Flags]
        public enum GizmoMode
        {
            Vertices = 1 << 1, 
            Normals = 1 << 2, 
            Tangents = 1 << 3,
            Triangles = 1 << 4
        }
        
        static readonly MeshJobScheduleDelegate[] _jobs = 
        {
            MeshJob<RowSquareGrid, SingleStreams>.ScheduleParallel,
            MeshJob<SharedSquareGrid, SingleStreams>.ScheduleParallel,
            MeshJob<SharedTriangleGrid, SingleStreams>.ScheduleParallel,
            MeshJob<PointyHexagonGrid, SingleStreams>.ScheduleParallel,
            MeshJob<FlatHexagonGrid, SingleStreams>.ScheduleParallel,
            MeshJob<CubeSphere, SingleStreams>.ScheduleParallel,
            MeshJob<SharedCubeSphere, PositionStreams>.ScheduleParallel,
            MeshJob<IcoSphere, PositionStreams>.ScheduleParallel,
            MeshJob<GeoIcoSphere, PositionStreams>.ScheduleParallel,
            MeshJob<OctaSphere, SingleStreams>.ScheduleParallel,
            MeshJob<GeoOctaSphere, SingleStreams>.ScheduleParallel,
            MeshJob<UVSphere, SingleStreams>.ScheduleParallel
        };

        public enum MeshType 
        {
            SquareGrid, 
            SharedSquareGrid, 
            SharedTriangleGrid, 
            PointyHexagonGrid, 
            FlatHexagonGrid, 
            CubeSphere,
            SharedCubeSphere,
            Icosphere,
            GeoIcoSphere,
            OctaSphere,
            GeoOctaSphere,
            UVSphere
        };

        [SerializeField] MeshType _meshType;

        [SerializeField, Range(1, 50)] int _resolution = 1;

        [SerializeField] private GizmoMode _gizmoMode;

        private UnityEngine.Mesh _mesh;

        [NonSerialized] private Vector3[] _vertices, _normals;
        [NonSerialized] private Vector4[] _tangents;
        [NonSerialized] private int[] _triangles;

        protected void Awake () 
        {
            _mesh = new UnityEngine.Mesh { name = "Procedural Mesh" };
            GetComponent<MeshFilter>().mesh = _mesh;
        }
        
        protected void OnValidate () => enabled = true;

        protected void Update () 
        {
            GetComponent<MeshRenderer>().material = _materials[(int)_materialMode];
            GenerateMesh();
            _vertices = null;
            _normals = null;
            _tangents = null;
            _triangles = null;
            enabled = false;
        }
        
        protected void OnDrawGizmos () 
        {
            if (_mesh == null || _gizmoMode == 0) 
            {
                return;
            }
            Transform t = transform;
            
            bool drawVertices = (_gizmoMode & GizmoMode.Vertices) != 0;
            bool drawNormals = (_gizmoMode & GizmoMode.Normals) != 0 && _mesh.HasVertexAttribute(VertexAttribute.Normal);
            bool drawTangents = (_gizmoMode & GizmoMode.Tangents) != 0 && _mesh.HasVertexAttribute(VertexAttribute.Tangent);
            bool drawTriangles = (_gizmoMode & GizmoMode.Triangles) != 0;
            
            _vertices ??= _mesh.vertices;
            if (drawNormals && _normals == null) 
            {
                _normals = _mesh.normals;
            }
            if (drawTangents && _tangents == null) 
            {
                _tangents = _mesh.tangents;
            }
            if (drawTriangles && _triangles == null) 
            {
                _triangles = _mesh.triangles;
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
                if (drawTangents)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(position, t.TransformDirection(_tangents[i] * 0.2f));   
                }
            }
            
            if (drawTriangles) 
            {
                float colorStep = 1f / (_triangles.Length - 3);
                for (int i = 0; i < _triangles.Length; i += 3) 
                {
                    float c = i * colorStep;
                    Gizmos.color = new Color(c, 0f, c);
                    Gizmos.DrawSphere(
                        t.TransformPoint((
                            _vertices[_triangles[i]] +
                            _vertices[_triangles[i + 1]] +
                            _vertices[_triangles[i + 2]]
                        ) * (1f / 3f)),
                        0.02f
                    );
                }
            }
        }

        private void GenerateMesh()
        {
            Debug.Log($"{this} {nameof(GenerateMesh)} with {nameof(_resolution)} {_resolution}");
            UnityEngine.Mesh.MeshDataArray meshDataArray = UnityEngine.Mesh.AllocateWritableMeshData(1);
            UnityEngine.Mesh.MeshData meshData = meshDataArray[0];
            _jobs[(int)_meshType](_mesh, meshData, _resolution, default).Complete();
            UnityEngine.Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, _mesh);
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
        }
    }
}