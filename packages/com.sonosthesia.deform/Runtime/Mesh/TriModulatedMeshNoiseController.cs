using System;
using System.Collections.Generic;
using Sonosthesia.Mesh;
using Sonosthesia.Noise;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace Sonosthesia.Deform
{
    public class TriModulatedMeshNoiseController : MeshNoiseController
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
            UnityEngine.Mesh.MeshData meshData, int resolution, 
            NativeArray<TriNoise.TriNoiseComponent> target, NativeArray<TriNoise.TriNoiseComponent> modulator,
            SpaceTRS domain, bool isPlane, ModulationStrategy modulationStrategy, JobHandle dependency
        );
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct Job<N> : IJobFor where N : struct, INoise, ISimpleNoise
        {
            [ReadOnly] private NativeArray<TriNoise.TriNoiseComponent> target;
            [ReadOnly] private NativeArray<TriNoise.TriNoiseComponent> modulator;
            
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
        
            public static JobHandle ScheduleParallel (UnityEngine.Mesh.MeshData meshData, int resolution, 
                NativeArray<TriNoise.TriNoiseComponent> target, NativeArray<TriNoise.TriNoiseComponent> modulator,
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

        [SerializeField] private SpaceTRS _domain = new SpaceTRS { scale = 1f };
        
        [SerializeField] private int _seed;
        
        [SerializeField] private List<DynamicNoiseSettings> _targetSettings;
        private NativeArray<TriNoise.TriNoiseComponent> _targetConfigs;
        private float[] _targetTimes;
        
        [SerializeField] private List<DynamicNoiseSettings> _modulatorSettings;
        private NativeArray<TriNoise.TriNoiseComponent> _modulatorConfigs;
        private float[] _modulatorTimes;

        [SerializeField] private ModulationStrategy _modulationStrategy;

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
                _targetConfigs = new NativeArray<TriNoise.TriNoiseComponent>(_targetSettings.Count, Allocator.Persistent);
            }

            if (_targetTimes == null || _targetTimes.Length != _targetSettings.Count)
            {
                _targetTimes = new float[_targetSettings.Count];
            }
            
            if (_modulatorConfigs.Length != _modulatorSettings.Count)
            {
                _modulatorConfigs.Dispose();
                _modulatorConfigs = new NativeArray<TriNoise.TriNoiseComponent>(_modulatorSettings.Count, Allocator.Persistent);
            }

            if (_modulatorTimes == null || _modulatorTimes.Length != _modulatorSettings.Count)
            {
                _modulatorTimes = new float[_modulatorSettings.Count];
            }
            
            base.OnValidate();
        }

        protected override JobHandle PerturbMesh(UnityEngine.Mesh.MeshData meshData, int resolution, float displacement, NoiseType noiseType, int dimensions, JobHandle dependency)
        {
            for (int i = 0; i < _targetSettings.Count; i++)
            {
                _targetConfigs[i] = TriNoise.GetNoiseComponent(_targetSettings[i], _seed, displacement, _targetTimes[i]);
            }
            
            for (int i = 0; i < _modulatorSettings.Count; i++)
            {
                _modulatorConfigs[i] = TriNoise.GetNoiseComponent(_modulatorSettings[i], _seed, displacement, _modulatorTimes[i]);
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