using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Sonosthesia.Noise;
using Sonosthesia.Ease;
using Sonosthesia.Mesh;

namespace Sonosthesia.Deform
{
    public enum Noise4DType
    {
        Simplex,
        Perlin
    }

    public readonly struct CompoundPathNoiseInfo
    {
        private const float IMPOTENCE_THRESHOLD = 1e-4f;
        
        public readonly Noise4DType noiseType;
        public readonly float displacement;
        public readonly EaseType falloffType;
        public readonly float3 center;
        public readonly float radius;
        public readonly float time;
        public readonly float3 offset;
        public readonly float frequency;
        
        public CompoundPathNoiseInfo(Noise4DType noiseType, float displacement,
            EaseType falloffType, float3 center, float radius, 
            float time, float3 offset, float frequency)
        {
            this.noiseType = noiseType;
            this.displacement = displacement;
            this.falloffType = falloffType;
            this.center = center;
            this.radius = radius;
            this.time = time;
            this.offset = offset;
            this.frequency = frequency;
        }

        public bool IsImpotent
        {
            get
            {
                if (math.abs(displacement) < IMPOTENCE_THRESHOLD)
                {
                    return true;
                }
                if (math.abs(radius) < IMPOTENCE_THRESHOLD)
                {
                    return true;
                }
                if (math.abs(frequency) < IMPOTENCE_THRESHOLD)
                {
                    return true;
                }

                return false;
            }
        }

        public override string ToString()
        {
            return $"{nameof(CompoundPathNoiseInfo)}(" +
                   $"{nameof(noiseType)}: {noiseType}, " +
                   $"{nameof(displacement)}: {displacement}, " +
                   $"{nameof(falloffType)}: {falloffType}, " +
                   $"{nameof(center)}: {center}, " +
                   $"{nameof(radius)}: {radius}, " +
                   $"{nameof(time)}: {time}, " +
                   $"{nameof(offset)}: {offset}, " +
                   $"{nameof(frequency)}: {frequency})";
        }
    }
    
    public class CompoundNoisePathProcessor : PathProcessor
    {
        [SerializeField] private Vector3 _direction = Vector3.up; 
        
        private readonly Dictionary<Guid, CompoundPathNoiseInfo> _components = new();

        private readonly UnsafeNativeArraySummationHelper<float> _summationHelper = new();

        private delegate JobHandle JobScheduleDelegate(
            CompoundPathNoiseInfo info,
            float3 localCenter,
            NativeArray<RigidTransform> points,
            NativeArray<float> deformations,
            int innerloopBatchCount,
            JobHandle dependency
        );

        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct Job<N> : IJobFor where N : struct, INoise4D
        {
            private CompoundPathNoiseInfo info;
            private float3 localCenter;
            private float radiusSqr;

            [ReadOnly] public NativeArray<RigidTransform> points;
            [WriteOnly] public NativeArray<float> deformations;

            public void Execute(int index)
            {
                float3 point = points[index].pos;
                // short cut uses sqr to avoid expensive sqrt
                float distanceSqr = math.distancesq(point, localCenter);
                if (distanceSqr > radiusSqr)
                {
                    deformations[index] = 0f;
                    return;
                }
                // now that we know we need it, compute sqrt
                float distance = math.sqrt(distanceSqr);
                float falloff = info.falloffType.Evaluate(math.unlerp(info.radius, 0, distance));
                N noise = default;
                float4 coord = new float4(point * info.frequency + info.offset, info.time);
                deformations[index] = noise.Compute(coord) * info.displacement * falloff;
            }

            public static JobHandle ScheduleParallel(CompoundPathNoiseInfo info, float3 localCenter,
                NativeArray<RigidTransform> points, NativeArray<float> deformations, 
                int innerloopBatchCount, JobHandle dependency)
            {
                return new Job<N>
                {
                    info = info,
                    points = points,
                    deformations = deformations,
                    radiusSqr = math.pow(info.radius, 2),
                    localCenter = localCenter
                }.ScheduleParallel(points.Length, innerloopBatchCount, dependency);
            }
        }

        private static readonly JobScheduleDelegate[] _delegates = 
        {
            Job<SNoise4D>.ScheduleParallel,
            Job<CNoise4D>.ScheduleParallel
        };

        public void Register(Guid id, CompoundPathNoiseInfo info)
        {
            if (info.IsImpotent)
            {
                Unregister(id);
            }
            else
            {
                if (!_components.ContainsKey(id))
                {
                    Debug.LogWarning($"{this} {nameof(Register)} (id {id}) : {info}");
                }
                
                _components[id] = info;
            }
        }

        public void Unregister(Guid id)
        {
            if (_components.Remove(id))
            {
                Debug.LogWarning($"{this} {nameof(Unregister)} (id {id})");   
            }
        }

        protected virtual void OnDestroy()
        {
            _summationHelper.Dispose();
        }

        public override void Process(NativeArray<RigidTransform> points)
        {
            if (points.Length == 0)
            {
                return;
            }
            
            _summationHelper.Length = points.Length;
            _summationHelper.ComponentCount = _components.Count;
            _summationHelper.Check();
            
            // Compute the deformations for each component in parallel
            
            NativeArray<JobHandle> deformationJobs = new NativeArray<JobHandle>(_summationHelper.ComponentCount, Allocator.Temp);
            int i = 0;
            foreach (CompoundPathNoiseInfo info in _components.Values)
            {
                float3 localCenter = transform.InverseTransformPoint(info.center);
                JobScheduleDelegate jobScheduleDelegate = _delegates[(int)info.noiseType];
                deformationJobs[i] = jobScheduleDelegate(info, localCenter, points, _summationHelper.terms[i], 100, default);
                i++;
            }
            
            // Sum the deformations in series, with the deformation jobs as dependency
            
            JobHandle summationDependency = _summationHelper.FloatSum(JobHandle.CombineDependencies(deformationJobs));
            
            // Apply the deformations to the path

            DeformPathJob deformPathJob = new DeformPathJob
            {
                points = points,
                deformations = _summationHelper.sum,
                direction = _direction
            };

            deformPathJob.ScheduleParallel(points.Length, 100, summationDependency).Complete();
            
            // Debug.Log($"{this} deformed using {string.Join("; ", _summationHelper.sum.Select(f => $"{f:F2}"))}");
        }
    }
}