using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Sonosthesia.Noise;

namespace Sonosthesia.Deform
{
    [RequireComponent(typeof(MeshRenderer))]
    public class AdditiveDeformationMeshController : SingleStreamMeshController
    {
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
            
            if (_components.Length == 0)
            {
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
            
            return ApplyMeshSampleDeformationJob.ScheduleParallel(data, 
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