using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace Sonosthesia.Builder
{
    public class TriAdvancedMeshNoiseController : CatlikeMeshNoiseController
    {
        [Serializable]
        protected struct AdvancedSettings
        {
            public int Frequency;
            public float Displacement;
            public float Velocity;
            public AnimationCurve LerpCurve;
        }
        
        protected readonly struct NoiseComponent
        {
            public readonly int Seed;
            public readonly float Displacement;

            public NoiseComponent(int seed, float displacement)
            {
                Seed = seed;
                Displacement = displacement;
            }

            public override string ToString()
            {
                return $"({Seed} : {Displacement})";
            }
        }
        
        protected readonly struct NoiseConfig
        {
            public readonly int Frequency;
            public readonly NoiseComponent C1;
            public readonly NoiseComponent C2;
            public readonly NoiseComponent C3;

            public NoiseConfig(int frequency, NoiseComponent c1, NoiseComponent c2, NoiseComponent c3)
            {
                Frequency = frequency;
                C1 = c1;
                C2 = c2;
                C3 = c3;
            }

            public override string ToString()
            {
                return $"({nameof(Frequency)} : {Frequency}, {nameof(C1)} : {C1}, {nameof(C2)} : {C2}, {nameof(C3)} : {C3})";
            }
        }
        
        private delegate JobHandle JobScheduleDelegate (
            Mesh.MeshData meshData, int resolution, NativeArray<NoiseConfig> configs, SpaceTRS domain,
            bool isPlane, JobHandle dependency
        );
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct Job<N> : IJobFor where N : struct, Noise.INoise
        {
            [ReadOnly] private NativeArray<NoiseConfig> configs;
            
            private float3x4 domainTRS;
            private float3x3 derivativeMatrix;
            private bool isPlane;
            private NativeArray<Vertex4> vertices;

            private Sample4 GetNoise(float4x3 position, NoiseComponent component, int frequency)
            {
                SmallXXHash4 hash = SmallXXHash4.Seed(component.Seed);
                return default(N).GetNoise4(position, hash, frequency) * component.Displacement;
            }
            
            public void Execute(int i)
            {
                Vertex4 v = vertices[i];

                float4x3 position = domainTRS.TransformVectors(transpose(float3x4(
                    v.v0.position, v.v1.position, v.v2.position, v.v3.position
                )));
                
                Sample4 noise = default;
                
                foreach (NoiseConfig config in configs)
                {
                    Sample4 noise1 = GetNoise(position, config.C1, config.Frequency);
                    noise1.Derivatives = derivativeMatrix.TransformVectors(noise1.Derivatives);
                
                    Sample4 noise2 = GetNoise(position, config.C2, config.Frequency);
                    noise2.Derivatives = derivativeMatrix.TransformVectors(noise2.Derivatives);
                
                    Sample4 noise3 = GetNoise(position, config.C3, config.Frequency);
                    noise3.Derivatives = derivativeMatrix.TransformVectors(noise3.Derivatives);

                    noise += noise1 + noise2 + noise3;
                }
                
                vertices[i] = SurfaceUtils.SetVertices(v, noise, isPlane);
            }
        
            public static JobHandle ScheduleParallel (Mesh.MeshData meshData, int resolution, 
                NativeArray<NoiseConfig> configs, SpaceTRS domain, bool isPlane, JobHandle dependency
            )
            {
                return new Job<N>
                {
                    vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
                    domainTRS = domain.Matrix,
                    derivativeMatrix = domain.DerivativeMatrix,
                    configs = configs,
                    isPlane = isPlane
                }.ScheduleParallel(meshData.vertexCount / 4, resolution, dependency);
            }
        }   
        
        private static JobScheduleDelegate[,] _jobs = {
            {
                Job<Noise.Lattice1D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel,
                Job<Noise.Lattice2D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel,
                Job<Noise.Lattice3D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel
            },
            {
                Job<Noise.Lattice1D<Noise.Smoothstep<Noise.Turbulence<Noise.Perlin>>, Noise.LatticeNormal>>.ScheduleParallel,
                Job<Noise.Lattice2D<Noise.Smoothstep<Noise.Turbulence<Noise.Perlin>>, Noise.LatticeNormal>>.ScheduleParallel,
                Job<Noise.Lattice3D<Noise.Smoothstep<Noise.Turbulence<Noise.Perlin>>, Noise.LatticeNormal>>.ScheduleParallel
            },
            {
                Job<Noise.Lattice1D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel,
                Job<Noise.Lattice2D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel,
                Job<Noise.Lattice3D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel
            },
            {
                Job<Noise.Simplex1D<Noise.Simplex>>.ScheduleParallel,
                Job<Noise.Simplex2D<Noise.Simplex>>.ScheduleParallel,
                Job<Noise.Simplex3D<Noise.Simplex>>.ScheduleParallel
            },
            {
                Job<Noise.Simplex1D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel,
                Job<Noise.Simplex2D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel,
                Job<Noise.Simplex3D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel
            },
            {
                Job<Noise.Simplex1D<Noise.Smoothstep<Noise.Turbulence<Noise.Simplex>>>>.ScheduleParallel,
                Job<Noise.Simplex2D<Noise.Smoothstep<Noise.Turbulence<Noise.Simplex>>>>.ScheduleParallel,
                Job<Noise.Simplex3D<Noise.Smoothstep<Noise.Turbulence<Noise.Simplex>>>>.ScheduleParallel
            },
            {
                Job<Noise.Simplex1D<Noise.Value>>.ScheduleParallel,
                Job<Noise.Simplex2D<Noise.Value>>.ScheduleParallel,
                Job<Noise.Simplex3D<Noise.Value>>.ScheduleParallel
            },
            {
                Job<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel,
                Job<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel,
                Job<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel
            },
            {
                Job<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel,
                Job<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel,
                Job<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel
            },
            {
                Job<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel,
                Job<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel,
                Job<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel
            },
            {
                Job<Noise.Voronoi1D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F1>>.ScheduleParallel,
                Job<Noise.Voronoi2D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F1>>.ScheduleParallel,
                Job<Noise.Voronoi3D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F1>>.ScheduleParallel
            },
            {
                Job<Noise.Voronoi1D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F2>>.ScheduleParallel,
                Job<Noise.Voronoi2D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F2>>.ScheduleParallel,
                Job<Noise.Voronoi3D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F2>>.ScheduleParallel
            },
            {
                Job<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel,
                Job<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel,
                Job<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel
            },
            {
                Job<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel,
                Job<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel,
                Job<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel
            },
            {
                Job<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel,
                Job<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel,
                Job<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel
            }
        };

        [SerializeField] private List<AdvancedSettings> _settings;
        
        private NativeArray<NoiseConfig> _noiseConfigs;
        private float[] _localTimes;

        protected override bool IsDynamic => true;

        protected override void Update()
        {
            for (int i = 0; i < _settings.Count; i++)
            {
                _localTimes[i] += Time.deltaTime * _settings[i].Velocity;
            }
            Debug.Log($"Local times is {string.Join(", ", _localTimes)}");
            base.Update();
        }
        
        protected override void OnValidate()
        {
            if (_noiseConfigs.Length != _settings.Count)
            {
                _noiseConfigs.Dispose();
                _noiseConfigs = new NativeArray<NoiseConfig>(_settings.Count, Allocator.Persistent);
            }

            if (_localTimes == null || _localTimes.Length != _settings.Count)
            {
                _localTimes = new float[_settings.Count];
                Debug.Log("Init _localTimes");
            }
            
            base.OnValidate();
        }
        
        private const float ONE_THIRD = 1f / 3f;
        
        protected virtual NoiseConfig GetNoiseConfig(AdvancedSettings settings, int seed, float displacement, float localTime)
        {
            NoiseComponent GetNoiseComponent(int index)
            {
                float time = localTime + index * ONE_THIRD;
                int flooredTime = Mathf.FloorToInt(time);
                return new NoiseComponent(
                    seed + flooredTime + index, 
                    settings.LerpCurve.Evaluate((time - flooredTime) * 2) * displacement * settings.Displacement);
            }

            return new NoiseConfig(
                settings.Frequency,
                GetNoiseComponent(0),
                GetNoiseComponent(1),
                GetNoiseComponent(2)
            );
        }
        
        protected override JobHandle PerturbMesh(Mesh.MeshData meshData, int resolution, float displacement, NoiseType noiseType, int dimensions, int seed, SpaceTRS domain, JobHandle dependency)
        {
            for (int i = 0; i < _settings.Count; i++)
            {
                _noiseConfigs[i] = GetNoiseConfig(_settings[i], seed, displacement, _localTimes[i]);
            }
            
            //Debug.Log($"Scheduling with configs {string.Join(",", _noiseConfigs)}");
            
            return _jobs[(int) noiseType, dimensions - 1](
                meshData,
                resolution,
                _noiseConfigs,
                domain,
                IsPlane,
                dependency);
        }
    }
}