using Sonosthesia.Mesh;
using Unity.Jobs;
using UnityEngine;

namespace Sonosthesia.Deform
{
    public abstract class DeformMeshController : MeshController
    {
        private static readonly int materialIsPlaneId = Shader.PropertyToID("_IsPlane");

        protected bool IsPlane => _meshType < MeshType.CubeSphere;

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

        private MeshType? _previousMeshType;
        private Material _material;
        private bool _setIsPlane;

        protected override void Awake()
        {
            base.Awake();
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                _material = meshRenderer.material;
                _setIsPlane = _material.HasFloat(materialIsPlaneId);   
            }
        }

        protected override void Update()
        {
            base.Update();
            if (_setIsPlane)
            {
                _material.SetFloat(materialIsPlaneId, IsPlane ? 1f : 0f);
            }
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


            JobHandle deformJob = DeformMesh(data, _resolution, _displacement, meshJob);
            
            deformJob.Complete();
        }

        protected abstract JobHandle DeformMesh(UnityEngine.Mesh.MeshData data, int resolution, float displacement, JobHandle dependency);
    }
}