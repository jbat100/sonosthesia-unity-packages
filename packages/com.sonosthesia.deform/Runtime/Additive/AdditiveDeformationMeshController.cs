using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Sonosthesia.Noise;
using Sonosthesia.Mesh;
using Unity.Mathematics;

namespace Sonosthesia.Deform
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class AdditiveDeformationMeshController : MeshController
    {
        private static int materialIsPlaneId = Shader.PropertyToID("_IsPlane");

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
        private NativeArray<Sample4>[] _deformations;
        private NativeArray<Sample4> _totalDeformation;
        private Material _material;
        private bool _setIsPlane;

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

                Debug.LogWarning($"{this} set {nameof(VertexCount)} from {_vertexCount} to {value}");
                
                _vertexCount = value;
                int length = Mathf.CeilToInt(_vertexCount / 4f);
                int componentCount = _components?.Length ?? 0;
                
                
                if (_deformations == null)
                {
                    _deformations = new NativeArray<Sample4>[componentCount];
                }
                else
                {
                    for (int i = 0; i < _deformations.Length; i++)
                    {
                        _deformations[i].Dispose();
                    }    
                }

                if (_deformations.Length != componentCount)
                {
                    _deformations = new NativeArray<Sample4>[componentCount];
                }

                _totalDeformation.Dispose();
                
                for (int i = 0; i < _deformations.Length; i++)
                {
                    _deformations[i] = new NativeArray<Sample4>(length, Allocator.Persistent);
                }

                _totalDeformation = new NativeArray<Sample4>(length, Allocator.Persistent);
            }
        }

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
            VertexCount = 0;
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

            VertexCount = data.vertexCount;

            if (VertexCount == 0)
            {
                Debug.LogWarning($"Unexpected vertex count {VertexCount} {data.vertexCount}");
                return;
            }

            if (_deformations == null)
            {
                // TODO : this happens on OnValidate rebuild call, work out why...
                Debug.LogWarning($"Unexpected null _deformations");
                return;
            }

            NativeArray<JobHandle> deformationJobs = new NativeArray<JobHandle>(_deformations.Length, Allocator.Temp);
            for (int i = 0; i < _deformations.Length; i++)
            {
                deformationJobs[i] = _components[i].MeshDeformation(data, _deformations[i], _resolution, default);
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

            JobHandle deformJob = ApplyMeshDeformationJob.ScheduleParallel(data,
                _totalDeformation, _displacement, IsPlane, _resolution, sumDependency);

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