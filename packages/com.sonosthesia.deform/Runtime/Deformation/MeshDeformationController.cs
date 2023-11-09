using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class MeshDeformationController : MonoBehaviour
    {
        private static int materialIsPlaneId = Shader.PropertyToID("_IsPlane");
        
        bool IsPlane => _meshType < MeshType.CubeSphere;
        
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
        
        [System.Flags]
        public enum GizmoMode
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

        [SerializeField] private GizmoMode _gizmoMode;
        
        [SerializeField] private SpaceTRS _domain = new SpaceTRS { scale = 1f };
        
        [SerializeField, Range(-1f, 1f)] private float _displacement = 0.5f;
        
        [SerializeField] private bool _recalculateNormals, _recalculateTangents;
        
        [SerializeField] private DeformationComponent[] _components;

        private MeshType? _previousMeshType;
        private NativeArray<Sample4>[] _deformations;
        private NativeArray<Sample4> _totalDeformation;
        private Mesh _mesh;

        private int _vertexCount = 0;
        private int VertexCount
        {
            get => _vertexCount;
            set
            {
                if (_vertexCount == value)
                {
                    return;
                }
                _vertexCount = value;
                int length = Mathf.CeilToInt(_vertexCount / 4f);
                for (int i = 0; i < _deformations.Length; i++)
                {
                    _deformations[i].Dispose();
                }
                _totalDeformation.Dispose();
                if (length > 0)
                {
                    for (int i = 0; i < _deformations.Length; i++)
                    {
                        _deformations[i] = new NativeArray<Sample4>(length, Allocator.Persistent);
                    }
                    _totalDeformation = new NativeArray<Sample4>(length, Allocator.Persistent);
                }
            }
        }
        
        protected void Awake()
        {
            _materials[(int)MaterialMode.Displacement] = new Material(_materials[(int)MaterialMode.Displacement]);
            _mesh = new Mesh { name = "Procedural Mesh" };
            GetComponent<MeshFilter>().mesh = _mesh;
            _deformations = new NativeArray<Sample4>[_components.Length];
        }

        protected void Update()
        {
            GenerateMesh();
            if (_materialMode == MaterialMode.Displacement) 
            {
                _materials[(int)MaterialMode.Displacement].SetFloat(
                    materialIsPlaneId, IsPlane ? 1f : 0f
                );
            }
            GetComponent<MeshRenderer>().material = _materials[(int)_materialMode];
        }

        protected void OnDestroy()
        {
            VertexCount = 0;
        }

        private void GenerateMesh()
        {
            Debug.Log($"{this} {nameof(GenerateMesh)} with {nameof(_resolution)} {_resolution}");
            Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData meshData = meshDataArray[0];

            JobHandle meshJob = _meshJobs[(int) _meshType](
                _mesh, 
                meshData, 
                _resolution, 
                default, 
                Vector3.one * Mathf.Abs(_displacement), 
                true);

            // we could schedule the mesh job in parallel with the deformation jobs but only if the deformation
            // array has the right size (i.e. mesh size has not changed since last update)
            meshJob.Complete();

            VertexCount = meshData.vertexCount;

            if (VertexCount == 0)
            {
                Debug.LogWarning($"Unexpected vertex count {VertexCount} {meshData.vertexCount}");
                return;
            }

            NativeArray<JobHandle> deformationJobs = new NativeArray<JobHandle>(_deformations.Length, Allocator.Temp);
            for (int i = 0; i < _deformations.Length; i++)
            {
                deformationJobs[i] = _components[i].GetDeformation(meshData, _deformations[i], _domain, Time.time, _resolution, default);
            }
            JobHandle.CombineDependencies(deformationJobs).Complete();

            _totalDeformation.ClearArray(_totalDeformation.Length);

            JobHandle sumDependency = default;
            for (int i = 0; i < _deformations.Length; i++)
            {
                sumDependency = new SumArrayJob<Sample4>()
                {
                    source = _deformations[i],
                    target = _totalDeformation
                }.ScheduleParallel(_totalDeformation.Length, _resolution, sumDependency);
            }

            JobHandle deformJob = DeformMeshJob.ScheduleParallel(meshData, 
                _totalDeformation, _domain, _displacement, IsPlane, _resolution, sumDependency);
            
            deformJob.Complete();
            
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