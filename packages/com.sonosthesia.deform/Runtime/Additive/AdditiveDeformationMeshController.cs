using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Sonosthesia.Noise;
using Sonosthesia.Mesh;
using Unity.Burst;

namespace Sonosthesia.Deform
{
    [RequireComponent(typeof(MeshRenderer))]
    public class AdditiveDeformationMeshController : DeformMeshController
    {
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct ApplyMeshDeformationJob : IJobFor
        {
            private float displacement;
            private bool isPlane;
            private NativeArray<Vertex4> vertices;
            private NativeArray<Sample4> deformations;

            public void Execute(int i)
            {
                Vertex4 v = vertices[i];
                Sample4 noise = deformations[i]  * displacement;
                vertices[i] = SurfaceUtils.SetVertices(v, noise, isPlane);
            }

            public static JobHandle ScheduleParallel (UnityEngine.Mesh.MeshData meshData, NativeArray<Sample4> deformations, 
                float displacement, bool isPlane, int innerloopBatchCount, JobHandle dependency
            ) => new ApplyMeshDeformationJob
            {
                vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
                deformations = deformations,
                displacement = displacement,
                isPlane = isPlane
            }.ScheduleParallel(meshData.vertexCount / 4, innerloopBatchCount, dependency);
        }    
        

        [SerializeField] private AdditiveDeformationComponent[] _components;

        private readonly UnsafeNativeArraySummationHelper<Sample4> _summationHelper = new ();
        
        protected virtual void OnDestroy()
        {
            _summationHelper.Dispose();
        }

        protected override JobHandle DeformMesh(UnityEngine.Mesh.MeshData data, int resolution, float displacement, JobHandle dependency)
        {
            if (data.vertexCount == 0)
            {
                Debug.LogWarning($"Unexpected vertex count {data.vertexCount} {data.vertexCount}");
                return dependency;
            }
            
            _summationHelper.Length = Mathf.CeilToInt(data.vertexCount / 4f);
            _summationHelper.ComponentCount = _components.Length;
            
            _summationHelper.Check();

            NativeArray<JobHandle> deformationJobs = new NativeArray<JobHandle>(_components.Length, Allocator.Temp);
            for (int i = 0; i < _components.Length; i++)
            {
                deformationJobs[i] = _components[i].MeshDeformation(data, _summationHelper.terms[i], resolution, dependency);
            }

            JobHandle sumDependency = _summationHelper.Sum(JobHandle.CombineDependencies(deformationJobs));
            
            return ApplyMeshDeformationJob.ScheduleParallel(data, 
                _summationHelper.sum, displacement, IsPlane, resolution, sumDependency);
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