using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace Sonosthesia.Builder
{
    public class TriDomainMeshNoiseController : CatlikeMeshNoiseController
    {
        private delegate JobHandle JobScheduleDelegate (
            Mesh.MeshData meshData, int resolution, NativeArray<TriNoise.DomainNoiseComponent> configs,
            bool isPlane, JobHandle dependency
        );
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct Job<N> : IJobFor where N : struct, Noise.INoise
        {
            [ReadOnly] private NativeArray<TriNoise.DomainNoiseComponent> configs;
            
            private bool isPlane;
            private NativeArray<Vertex4> vertices;

            public void Execute(int i)
            {
                Vertex4 v = vertices[i];
                Sample4 noise = default;
                for (int c = 0; c < configs.Length; c++)
                {
                    TriNoise.DomainNoiseComponent component = configs[c];
                    float4x3 position = component.DomainTRS.TransformVectors(transpose(float3x4(
                        v.v0.position, v.v1.position, v.v2.position, v.v3.position
                    )));
                    noise += position.GetNoise<N>(component.Component, component.DerivativeMatrix);
                }
                vertices[i] = SurfaceUtils.SetVertices(v, noise, isPlane);
            }
        
            public static JobHandle ScheduleParallel (Mesh.MeshData meshData, int resolution, 
                NativeArray<TriNoise.DomainNoiseComponent> configs, bool isPlane, JobHandle dependency
            )
            {
                return new Job<N>
                {
                    vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
                    configs = configs,
                    isPlane = isPlane
                }.ScheduleParallel(meshData.vertexCount / 4, resolution, dependency);
            }
        }   
        
        private static readonly JobScheduleDelegate[,] _jobs = {
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

        public DomainDynamicSettings GetSettings(int index) => _settings[index];

        [SerializeField] private List<DomainDynamicSettings> _settings;
        private NativeArray<TriNoise.DomainNoiseComponent> _noiseConfigs;
        private float[] _localTimes;

        protected override bool IsDynamic => true;

        protected override void Update()
        {
            for (int i = 0; i < _settings.Count; i++)
            {
                _localTimes[i] += Time.deltaTime * _settings[i].Settings.Velocity;
            }
            Debug.Log($"Local times is {string.Join(", ", _localTimes)}");
            base.Update();
        }

        protected virtual void OnEnable()
        {
            CheckArrays();
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

        protected override JobHandle PerturbMesh(Mesh.MeshData meshData, int resolution, float displacement, NoiseType noiseType, int dimensions, int seed, JobHandle dependency)
        {
            for (int i = 0; i < _settings.Count; i++)
            {
                DomainDynamicSettings settings = _settings[i];
                _noiseConfigs[i] = new TriNoise.DomainNoiseComponent(
                    TriNoise.GetNoiseComponent(settings.Settings, seed, displacement, _localTimes[i]),
                    settings.Domain.Matrix,
                    settings.Domain.DerivativeMatrix
                    );
            }
            
            Debug.Log($"Scheduling with configs {string.Join(",", _noiseConfigs)}");
            
            return _jobs[(int) noiseType, dimensions - 1](
                meshData,
                resolution,
                _noiseConfigs,
                IsPlane,
                dependency);
        }
    }
}