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
    public class TriModulatedMeshNoiseController : CatlikeMeshNoiseController
    {
        [Serializable]
        private struct ModulationStrategy
        {
            public bool AbsoluteValue;
            public float Threshold;

            private float4 Process(float4 m)
            {
                if (AbsoluteValue)
                {
                    m = abs(m);
                }
                if (Threshold > 0)
                {
                    m = select(m - Threshold, 0, m > Threshold);
                }
                return m;
            }
            
            public Sample4 Modulate(Sample4 s, float4 m)
            {
                s.v *= Process(m) ;
                return s;
            }
            
            public float4 Modulate(float4 v, float4 m)
            {
                return v * Process(m);
            }
        }
        
        private delegate JobHandle JobScheduleDelegate (
            Mesh.MeshData meshData, int resolution, 
            NativeArray<TriNoise.NoiseComponent> target, NativeArray<TriNoise.NoiseComponent> modulator,
            SpaceTRS domain, bool isPlane, ModulationStrategy modulationStrategy, JobHandle dependency
        );
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct Job<N> : IJobFor where N : struct, Noise.INoise, Noise.ISimpleNoise
        {
            [ReadOnly] private NativeArray<TriNoise.NoiseComponent> target;
            [ReadOnly] private NativeArray<TriNoise.NoiseComponent> modulator;
            
            private float3x4 domainTRS;
            private float3x3 derivativeMatrix;
            private bool isPlane;
            private ModulationStrategy modulationStrategy;
            private NativeArray<Vertex4> vertices;
            
            public void Execute(int i)
            {
                Vertex4 v = vertices[i];

                float4x3 position = domainTRS.TransformVectors(transpose(float3x4(
                    v.v0.position, v.v1.position, v.v2.position, v.v3.position
                )));
                
                Sample4 source = default;
                for (int c = 0; c < target.Length; c++)
                {
                    source += position.GetNoise<N>(target[c], derivativeMatrix);
                }

                float4 mod = default;
                for (int c = 0; c < modulator.Length; c++)
                {
                    mod += position.GetSimpleNoise<N>(modulator[c]);
                }
                
                vertices[i] = SurfaceUtils.SetVertices(v, modulationStrategy.Modulate(source, mod), isPlane);
            }
        
            public static JobHandle ScheduleParallel (Mesh.MeshData meshData, int resolution, 
                NativeArray<TriNoise.NoiseComponent> target, NativeArray<TriNoise.NoiseComponent> modulator,
                SpaceTRS domain, bool isPlane, ModulationStrategy modulationStrategy, JobHandle dependency
            )
            {
                return new Job<N>
                {
                    vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
                    domainTRS = domain.Matrix,
                    derivativeMatrix = domain.DerivativeMatrix,
                    target = target,
                    modulator = modulator,
                    isPlane = isPlane,
                    modulationStrategy = modulationStrategy
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

        [SerializeField] private SpaceTRS _domain = new SpaceTRS { scale = 1f };
        
        [SerializeField] private List<DynamicSettings> _targetSettings;
        private NativeArray<TriNoise.NoiseComponent> _targetConfigs;
        private float[] _targetTimes;
        
        [SerializeField] private List<DynamicSettings> _modulatorSettings;
        private NativeArray<TriNoise.NoiseComponent> _modulatorConfigs;
        private float[] _modulatorTimes;

        [SerializeField] private ModulationStrategy _modulationStrategy;

        protected override bool IsDynamic => true;

        protected override void Update()
        {
            for (int i = 0; i < _targetSettings.Count; i++)
            {
                _targetTimes[i] += Time.deltaTime * _targetSettings[i].Velocity;
            }
            for (int i = 0; i < _modulatorSettings.Count; i++)
            {
                _modulatorTimes[i] += Time.deltaTime * _modulatorSettings[i].Velocity;
            }
            base.Update();
        }
        
        protected override void OnValidate()
        {
            if (_targetConfigs.Length != _targetSettings.Count)
            {
                _targetConfigs.Dispose();
                _targetConfigs = new NativeArray<TriNoise.NoiseComponent>(_targetSettings.Count, Allocator.Persistent);
            }

            if (_targetTimes == null || _targetTimes.Length != _targetSettings.Count)
            {
                _targetTimes = new float[_targetSettings.Count];
            }
            
            if (_modulatorConfigs.Length != _modulatorSettings.Count)
            {
                _modulatorConfigs.Dispose();
                _modulatorConfigs = new NativeArray<TriNoise.NoiseComponent>(_modulatorSettings.Count, Allocator.Persistent);
            }

            if (_modulatorTimes == null || _modulatorTimes.Length != _modulatorSettings.Count)
            {
                _modulatorTimes = new float[_modulatorSettings.Count];
            }
            
            base.OnValidate();
        }

        protected override JobHandle PerturbMesh(Mesh.MeshData meshData, int resolution, float displacement, NoiseType noiseType, int dimensions, int seed, JobHandle dependency)
        {
            for (int i = 0; i < _targetSettings.Count; i++)
            {
                _targetConfigs[i] = TriNoise.GetNoiseComponent(_targetSettings[i], seed, displacement, _targetTimes[i]);
            }
            
            for (int i = 0; i < _modulatorSettings.Count; i++)
            {
                _modulatorConfigs[i] = TriNoise.GetNoiseComponent(_modulatorSettings[i], seed, displacement, _modulatorTimes[i]);
            }
            
            //Debug.Log($"Scheduling with configs {string.Join(",", _noiseConfigs)}");
            
            return _jobs[(int) noiseType, dimensions - 1](
                meshData,
                resolution,
                _targetConfigs,
                _modulatorConfigs,
                _domain,
                IsPlane,
                _modulationStrategy,
                dependency);
        }
    }
}