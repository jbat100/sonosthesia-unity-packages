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
    public class TriDomainMeshNoiseController : CatlikeMeshNoiseController
    {
        private delegate JobHandle JobScheduleDelegate (
            UnityEngine.Mesh.MeshData meshData, int resolution, NativeArray<TriNoise.DomainNoiseComponent> configs,
            bool isPlane, JobHandle dependency
        );
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct Job<N> : IJobFor where N : struct, INoise
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
        
            public static JobHandle ScheduleParallel (UnityEngine.Mesh.MeshData meshData, int resolution, 
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
            //Debug.Log($"Local times is {string.Join(", ", _localTimes)}");
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
                //Debug.Log("Init _localTimes");
            }
        }

        protected override JobHandle PerturbMesh(UnityEngine.Mesh.MeshData meshData, int resolution, float displacement, NoiseType noiseType, int dimensions, int seed, JobHandle dependency)
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
            
            //Debug.Log($"Scheduling with configs {string.Join(",", _noiseConfigs)}");
            
            return _jobs[(int) noiseType, dimensions - 1](
                meshData,
                resolution,
                _noiseConfigs,
                IsPlane,
                dependency);
        }
    }
}