using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Sonosthesia.Noise;
using Sonosthesia.Mesh;

namespace Sonosthesia.Deform
{
    [RequireComponent(typeof(MeshRenderer))]
    public class AdditiveDeformationMeshController : MeshController
    {
        private static readonly int materialIsPlaneId = Shader.PropertyToID("_IsPlane");

        bool IsPlane => _meshType < MeshType.CubeSphere;

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

        [SerializeField] private MeshType _meshType;

        [SerializeField, Range(1, 50)] private int _resolution = 1;

        [SerializeField, Range(-1f, 1f)] private float _displacement = 0.5f;

        [SerializeField] private AdditiveDeformationComponent[] _components;

        private MeshType? _previousMeshType;
        private Material _material;
        private bool _setIsPlane;

        private readonly UnsafeNativeArraySummationHelper<Sample4> _summationHelper = new ();

        protected override void Awake()
        {
            base.Awake();
            _material = GetComponent<MeshRenderer>().material;
            _setIsPlane = _material.HasFloat(materialIsPlaneId);
        }

        protected override void Update()
        {
            base.Update();
            if (_setIsPlane)
            {
                _material.SetFloat(materialIsPlaneId, IsPlane ? 1f : 0f);
            }
        }

        protected void OnDestroy()
        {
            _summationHelper.Dispose();
        }

        protected override void PopulateMeshData(UnityEngine.Mesh.MeshData data)
        {
            JobHandle meshJob = _meshJobs[(int)_meshType](
                Mesh,
                data,
                _resolution,
                default,
                Vector3.one * Mathf.Abs(_displacement),
                true);

            // we could schedule the mesh job in parallel with the deformation jobs but only if the deformation
            // array has the right size (i.e. mesh size has not changed since last update)

            meshJob.Complete();

            _summationHelper.Length = Mathf.CeilToInt(data.vertexCount / 4f);
            _summationHelper.ComponentCount = _components.Length;
            
            _summationHelper.Check();
            
            if (data.vertexCount == 0)
            {
                Debug.LogWarning($"Unexpected vertex count {data.vertexCount} {data.vertexCount}");
                return;
            }
            
            NativeArray<JobHandle> deformationJobs = new NativeArray<JobHandle>(_components.Length, Allocator.Temp);
            for (int i = 0; i < _components.Length; i++)
            {
                deformationJobs[i] = _components[i].MeshDeformation(data, _summationHelper.terms[i], _resolution, default);
            }

            JobHandle sumDependency = _summationHelper.Sum(JobHandle.CombineDependencies(deformationJobs));
            
            JobHandle deformJob = ApplyMeshDeformationJob.ScheduleParallel(data,
                _summationHelper.sum, _displacement, IsPlane, _resolution, sumDependency);

            deformJob.Complete();
        }

        public Sample4 ComputeDeformation(float3x4 vertex)
        {
            Sample4 deformation = default;
            foreach (AdditiveDeformationComponent component in _components)
            {
                deformation += component.VertexDeformation(vertex);
            }

            return deformation;
        }
    }
}