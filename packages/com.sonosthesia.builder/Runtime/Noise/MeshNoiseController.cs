using System;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace Sonosthesia.Builder
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public abstract class MeshNoiseController : MonoBehaviour
    {
        private static int materialIsPlaneId = Shader.PropertyToID("_IsPlane");
        
        protected bool IsPlane => _meshType < MeshType.CubeSphere;

        protected virtual bool IsDynamic => false;
        
        [System.Flags]
        public enum MeshOptimizationMode 
        {
            ReorderIndices = 1 << 0, 
            ReorderVertices = 1 << 1
        }

        [SerializeField] private MeshOptimizationMode _meshOptimization;
        
        public enum MaterialMode
        {
            Displacement,
            Flat,
            LatLonMap,
            CubeMap
        }

        [SerializeField] private MaterialMode _materialMode;
        
        [SerializeField] private Material[] _materials;
        
        [Flags]
        private enum GizmoMode
        {
            Vertices = 1 << 1, 
            Normals = 1 << 2, 
            Tangents = 1 << 3,
            Triangles = 1 << 4
        }
        
        // changed to use only SingleStreams to simplify deformation code
        
        static readonly AdvancedMeshJobScheduleDelegate[] _meshJobs = 
        {
            MeshJob<RowSquareGrid, SingleStreams>.ScheduleParallel,
            MeshJob<SharedSquareGrid, SingleStreams>.ScheduleParallel,
            MeshJob<SharedTriangleGrid, SingleStreams>.ScheduleParallel,
            MeshJob<PointyHexagonGrid, SingleStreams>.ScheduleParallel,
            MeshJob<FlatHexagonGrid, SingleStreams>.ScheduleParallel,
            MeshJob<CubeSphere, SingleStreams>.ScheduleParallel,
            MeshJob<SharedCubeSphere, SingleStreams>.ScheduleParallel,
            MeshJob<IcoSphere, SingleStreams>.ScheduleParallel,
            MeshJob<GeoIcoSphere, SingleStreams>.ScheduleParallel,
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
        
        [SerializeField, Range(-1f, 1f)] private float _displacement = 0.5f;

        [SerializeField] private GizmoMode _gizmoMode;

        [SerializeField] private bool _recalculateNormals, _recalculateTangents;
        
        private Mesh _mesh;
        private Vector3[] _vertices, _normals;
        private Vector4[] _tangents;
        private int[] _triangles;
        private MeshRenderer _renderer;

        protected void Awake ()
        {
            _renderer = GetComponent<MeshRenderer>();
            _materials[(int)MaterialMode.Displacement] = new Material(_materials[(int)MaterialMode.Displacement]);
            _mesh = new Mesh { name = "Procedural Mesh" };
            GetComponent<MeshFilter>().mesh = _mesh;
        }
        
        protected virtual void OnValidate () => enabled = true;

        protected virtual void Update () 
        {
            GenerateMesh();
            _vertices = null;
            _normals = null;
            _tangents = null;
            _triangles = null;

            if (!IsDynamic)
            {
                enabled = false;   
            }

            if (_materialMode == MaterialMode.Displacement) 
            {
                _materials[(int)MaterialMode.Displacement].SetFloat(
                    materialIsPlaneId, IsPlane ? 1f : 0f
                );
            }
            
            _renderer.material = _materials[(int)_materialMode];
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

        protected abstract JobHandle PerturbMesh(Mesh.MeshData meshData, int resolution, float displacement, JobHandle dependency);

        private void GenerateMesh()
        {
            //Debug.Log($"{this} {nameof(GenerateMesh)} with {nameof(_resolution)} {_resolution}");
            Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData meshData = meshDataArray[0];

            JobHandle meshJob = _meshJobs[(int) _meshType](_mesh, meshData, _resolution, default, Vector3.one * Mathf.Abs(_displacement), true);

            PerturbMesh(meshData, _resolution, _displacement, meshJob).Complete();

            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, _mesh);

            if (_recalculateNormals)
            {
                _mesh.RecalculateNormals();
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
        }
    }
}