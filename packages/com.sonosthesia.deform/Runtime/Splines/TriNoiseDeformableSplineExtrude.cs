using System.Collections.Generic;
using Sonosthesia.Noise;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

using static Unity.Mathematics.math;

namespace Sonosthesia.Deform
{
    public class TriNoiseDeformableSplineExtrude : NoiseDeformableSplineExtrude
    {
        private delegate JobHandle JobScheduleDelegate (
            UnityEngine.Mesh.MeshData meshData, int innerloopBatchCount, NativeArray<TriNoise.DomainNoiseComponent> configs,
            JobHandle dependency
        );
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct Job<N> : IJobFor where N : struct, INoise
        {
            [ReadOnly] private NativeArray<TriNoise.DomainNoiseComponent> configs;
            
            private NativeArray<SplineVertexData4> vertices;

            public void Execute(int i)
            {
                SplineVertexData4 v = vertices[i];
                Sample4 noise = default;
                for (int c = 0; c < configs.Length; c++)
                {
                    TriNoise.DomainNoiseComponent component = configs[c];
                    float4x3 position = component.DomainTRS.TransformVectors(transpose(float3x4(
                        v.v0.position, v.v1.position, v.v2.position, v.v3.position
                    )));
                    noise += position.GetNoise<N>(component.Component, component.DerivativeMatrix);
                }
                vertices[i] = SplineUtils.DeformVerticesAlongNormals(v, noise);
            }
        
            public static JobHandle ScheduleParallel (UnityEngine.Mesh.MeshData meshData, int innerloopBatchCount, 
                NativeArray<TriNoise.DomainNoiseComponent> configs, JobHandle dependency
            )
            {
                return new Job<N>
                {
                    vertices = meshData.GetVertexData<SplineVertexData>().Reinterpret<SplineVertexData4>(12 + 12 + 8),
                    configs = configs
                }.ScheduleParallel(meshData.vertexCount / 4, innerloopBatchCount, dependency);
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
        
        [SerializeField, Range(-0.1f, 0.1f)] private float _displacement = 0.01f;
        
        public DomainDynamicSettings GetSettings(int index) => _settings[index];

        [SerializeField] private List<DomainDynamicSettings> _settings;
        private NativeArray<TriNoise.DomainNoiseComponent> _noiseConfigs;
        private float[] _localTimes;
        
        protected override void Update()
        {
            for (int i = 0; i < _settings.Count; i++)
            {
                _localTimes[i] += Time.deltaTime * _settings[i].Settings.Velocity;
            }
            base.Update();
        }
        
        protected override void OnEnable()
        {
            CheckArrays();
            base.OnEnable();
        }

        protected override void OnValidate()
        {
            CheckArrays();
            base.OnValidate();
        }
        
        private void CheckArrays()
        {
            if (_noiseConfigs.Length != _settings.Count)
            {
                _noiseConfigs.Dispose();
                _noiseConfigs = new NativeArray<TriNoise.DomainNoiseComponent>(_settings.Count, Allocator.Persistent);
            }

            if (_localTimes == null || _localTimes.Length != _settings.Count)
            {
                _localTimes = new float[_settings.Count];
                Debug.Log("Init _localTimes");
            }
        }

        protected override void Deform(ISpline spline, UnityEngine.Mesh.MeshData data, float radius, int sides, float segmentsPerUnit, 
            bool capped, float2 range, NoiseType noiseType, int dimensions, int seed)
        {
            for (int i = 0; i < _settings.Count; i++)
            {
                DomainDynamicSettings settings = _settings[i];
                _noiseConfigs[i] = new TriNoise.DomainNoiseComponent(
                    TriNoise.GetNoiseComponent(settings.Settings, seed, _displacement, _localTimes[i]),
                    settings.Domain.Matrix,
                    settings.Domain.DerivativeMatrix
                );
            }
            
            int innerloopBatchCount = (int)sqrt(segmentsPerUnit);
            
            _jobs[(int) noiseType, dimensions - 1](
                data,
                innerloopBatchCount,
                _noiseConfigs,
                default).Complete();
        }
    }
}