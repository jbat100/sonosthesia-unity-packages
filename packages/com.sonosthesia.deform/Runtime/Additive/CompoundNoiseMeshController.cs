using System;
using System.Collections.Generic;
using Sonosthesia.Ease;
using Sonosthesia.Mesh;
using Sonosthesia.Noise;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Deform
{
    public readonly struct CompoundMeshNoiseInfo
    {
        public readonly EaseType crossFadeType;
        public readonly CatlikeNoiseType noiseType;
        public readonly float displacement;
        public readonly float3x4 domainTRS;
        public readonly bool falloff;
        public readonly EaseType falloffType;
        public readonly float3 center;
        public readonly float radius;
        public readonly float time;
        public readonly int frequency;

        public static CompoundMeshNoiseInfo Global(CatlikeNoiseType noiseType, 
            float displacement, float3x4 domainTRS,
            float time, int frequency, EaseType crossFadeType = EaseType.easeInOutSine)
        {
            return new CompoundMeshNoiseInfo(crossFadeType, noiseType, displacement, domainTRS,
                false, EaseType.linear, float3.zero, 0f,
                time, frequency);
        }

        public static CompoundMeshNoiseInfo Local(CatlikeNoiseType noiseType, 
            float displacement, float3x4 domainTRS,
            EaseType falloffType, float3 center, float radius,
            float time, int frequency, EaseType crossFadeType = EaseType.easeInOutSine)
        {
            return new CompoundMeshNoiseInfo(crossFadeType, noiseType, displacement, domainTRS,
                true, falloffType, center, radius,
                time, frequency);
        }
        
        public CompoundMeshNoiseInfo(EaseType crossFadeType, CatlikeNoiseType noiseType, 
            float displacement, float3x4 domainTRS,
            bool falloff, EaseType falloffType, float3 center, float radius, 
            float time, int frequency)
        {
            this.crossFadeType = crossFadeType;
            this.noiseType = noiseType;
            this.displacement = displacement;
            this.domainTRS = domainTRS;
            this.falloff = falloff;
            this.falloffType = falloffType;
            this.center = center;
            this.radius = radius;
            this.time = time;
            this.frequency = frequency;
        }
        
        public override string ToString()
        {
            return $"{nameof(CompoundMeshNoiseInfo)} " +
                   $"{nameof(noiseType)}: {noiseType}, " +
                   $"{nameof(displacement)}: {displacement}, " +
                   $"{nameof(falloff)}: {falloff}, " +
                   $"{nameof(falloffType)}: {falloffType}, " +
                   $"{nameof(center)}: {center}, " +
                   $"{nameof(radius)}: {radius}, " +
                   $"{nameof(time)}: {time}, " +
                   $"{nameof(frequency)}: {frequency}";
        }
    }

    public class CompoundNoiseMeshController : DeformMeshController
    {
        private readonly Dictionary<Guid, CompoundMeshNoiseInfo> _components = new();

        private readonly UnsafeNativeArraySummationHelper<float4> _summationHelper = new();

        private delegate JobHandle JobScheduleDelegate (
            UnityEngine.Mesh.MeshData meshData, 
            NativeArray<float4> deformations, TriNoise.TriNoiseComponent component, 
            CompoundMeshNoiseInfo info, float3 localCenter,
            int innerloopBatchCount, JobHandle dependency
        );
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct Job<N> : IJobFor where N : struct, ISimpleNoise
        {
            [ReadOnly] private NativeArray<Vertex4> vertices;
            [WriteOnly] private NativeArray<float4> deformations;
            private TriNoise.TriNoiseComponent component;
            private CompoundMeshNoiseInfo info;
            private float3 localCenter;

            private float Falloff(float3 pos)
            {
                float distance = math.distance(pos, localCenter);
                float fade = math.clamp(math.unlerp(info.radius, 0, distance), 0, 1);
                float falloff = info.falloffType.Evaluate(fade);
                return falloff;
            }
            
            public void Execute(int i)
            {
                Vertex4 v = vertices[i];
                float4x3 position = info.domainTRS.TransformVectors(math.transpose(new float3x4(
                    v.v0.position, v.v1.position, v.v2.position, v.v3.position
                )));
                float4 noise = position.GetSimpleNoise<N>(component);
                if (info.falloff)
                {
                    //float4 distance = new float4(
                    //    math.distance(v.v3.position, localCenter),
                    //    math.distance(v.v2.position, localCenter),
                    //    math.distance(v.v1.position, localCenter),
                    //    math.distance(v.v0.position, localCenter)
                    //    );
                    //float4 fade = math.clamp(math.unlerp(info.radius, 0, distance), 0, 1);
                    //float4 falloff = info.falloffType.Evaluate(fade);
                    //noise = math.mul(noise, falloff);

                    noise.x *= Falloff(v.v0.position);
                    noise.y *= Falloff(v.v1.position);
                    noise.z *= Falloff(v.v2.position);
                    noise.w *= Falloff(v.v3.position);
                }
                deformations[i] = noise;
            }

            public static JobHandle ScheduleParallel (UnityEngine.Mesh.MeshData meshData, 
                NativeArray<float4> deformations, TriNoise.TriNoiseComponent component, 
                CompoundMeshNoiseInfo info, float3 localCenter,
                int innerloopBatchCount, JobHandle dependency)
            {
                return new Job<N>
                {
                    vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
                    deformations = deformations,
                    component = component,
                    info = info,
                    localCenter = localCenter
                }.ScheduleParallel(meshData.vertexCount / 4, innerloopBatchCount, dependency);
            }
        }
        
        private static readonly JobScheduleDelegate[,] _jobs = {
            {
                Job<Lattice1D<Perlin, LatticeNormal>>.ScheduleParallel,
                Job<Lattice2D<Perlin, LatticeNormal>>.ScheduleParallel,
                Job<Lattice3D<Perlin, LatticeNormal>>.ScheduleParallel
            },
            {
                Job<Lattice1D<Smoothstep<Turbulence<Perlin>>, LatticeNormal>>.ScheduleParallel,
                Job<Lattice2D<Smoothstep<Turbulence<Perlin>>, LatticeNormal>>.ScheduleParallel,
                Job<Lattice3D<Smoothstep<Turbulence<Perlin>>, LatticeNormal>>.ScheduleParallel
            },
            {
                Job<Lattice1D<Value, LatticeNormal>>.ScheduleParallel,
                Job<Lattice2D<Value, LatticeNormal>>.ScheduleParallel,
                Job<Lattice3D<Value, LatticeNormal>>.ScheduleParallel
            },
            {
                Job<Simplex1D<Simplex>>.ScheduleParallel,
                Job<Simplex2D<Simplex>>.ScheduleParallel,
                Job<Simplex3D<Simplex>>.ScheduleParallel
            },
            {
                Job<Simplex1D<Turbulence<Simplex>>>.ScheduleParallel,
                Job<Simplex2D<Turbulence<Simplex>>>.ScheduleParallel,
                Job<Simplex3D<Turbulence<Simplex>>>.ScheduleParallel
            },
            {
                Job<Simplex1D<Smoothstep<Turbulence<Simplex>>>>.ScheduleParallel,
                Job<Simplex2D<Smoothstep<Turbulence<Simplex>>>>.ScheduleParallel,
                Job<Simplex3D<Smoothstep<Turbulence<Simplex>>>>.ScheduleParallel
            },
            {
                Job<Simplex1D<Value>>.ScheduleParallel,
                Job<Simplex2D<Value>>.ScheduleParallel,
                Job<Simplex3D<Value>>.ScheduleParallel
            },
            {
                Job<Voronoi1D<LatticeNormal, Worley, F1>>.ScheduleParallel,
                Job<Voronoi2D<LatticeNormal, Worley, F1>>.ScheduleParallel,
                Job<Voronoi3D<LatticeNormal, Worley, F1>>.ScheduleParallel
            },
            {
                Job<Voronoi1D<LatticeNormal, Worley, F2>>.ScheduleParallel,
                Job<Voronoi2D<LatticeNormal, Worley, F2>>.ScheduleParallel,
                Job<Voronoi3D<LatticeNormal, Worley, F2>>.ScheduleParallel
            },
            {
                Job<Voronoi1D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
                Job<Voronoi2D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
                Job<Voronoi3D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel
            },
            {
                Job<Voronoi1D<LatticeNormal, SmoothWorley, F1>>.ScheduleParallel,
                Job<Voronoi2D<LatticeNormal, SmoothWorley, F1>>.ScheduleParallel,
                Job<Voronoi3D<LatticeNormal, SmoothWorley, F1>>.ScheduleParallel
            },
            {
                Job<Voronoi1D<LatticeNormal, SmoothWorley, F2>>.ScheduleParallel,
                Job<Voronoi2D<LatticeNormal, SmoothWorley, F2>>.ScheduleParallel,
                Job<Voronoi3D<LatticeNormal, SmoothWorley, F2>>.ScheduleParallel
            },
            {
                Job<Voronoi1D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel,
                Job<Voronoi2D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel,
                Job<Voronoi3D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel
            },
            {
                Job<Voronoi1D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel,
                Job<Voronoi2D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel,
                Job<Voronoi3D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel
            },
            {
                Job<Voronoi1D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel,
                Job<Voronoi2D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel,
                Job<Voronoi3D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel
            }
        };
        
        public void Register(Guid id, CompoundMeshNoiseInfo info)
        {
            _components[id] = info;
            // Debug.Log($"{this} {nameof(Register)} (id {id}) : {info}");
            _summationHelper.ComponentCount = _components.Count;
        }
        
        public void Unregister(Guid id)
        {
            _components.Remove(id);
            // Debug.Log($"{this} {nameof(Unregister)} (id {id})");
            _summationHelper.ComponentCount = _components.Count;
        }
        
        protected override JobHandle DeformMesh(UnityEngine.Mesh.MeshData data, int resolution, float displacement, JobHandle dependency)
        {
            if (data.vertexCount == 0)
            {
                Debug.LogWarning($"Unexpected vertex count {data.vertexCount} {data.vertexCount}");
                return dependency;
            }
            
            if (_components.Count == 0)
            {
                return dependency;
            }
            
            _summationHelper.Length = Mathf.CeilToInt(data.vertexCount / 4f);
            _summationHelper.ComponentCount = _components.Count;
            
            _summationHelper.Check();

            NativeArray<JobHandle> deformationJobs = new NativeArray<JobHandle>(_components.Count, Allocator.Temp);
            int i = 0;
            int dimensionIndex = IsPlane ? 0 : 2;
            foreach (CompoundMeshNoiseInfo info in _components.Values)
            {
                float3 localCenter = transform.InverseTransformPoint(info.center);
                JobScheduleDelegate deformationDelegate = _jobs[(int)info.noiseType, dimensionIndex];
                TriNoise.TriNoiseComponent component = TriNoise.GetNoiseComponent(info, 0);
                deformationJobs[i] = deformationDelegate(data, _summationHelper.terms[i], component, info, localCenter, resolution, dependency);
                i++;
            }

            JobHandle sumDependency = _summationHelper.Float4Sum(JobHandle.CombineDependencies(deformationJobs));
            
            return ApplyMeshFloatDeformationJob.ScheduleParallel(data, 
                _summationHelper.sum, displacement, IsPlane, resolution, sumDependency);
        }
    }
}