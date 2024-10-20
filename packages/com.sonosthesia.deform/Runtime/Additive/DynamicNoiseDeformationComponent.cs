using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Sonosthesia.Mesh;
using Sonosthesia.Noise;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Sonosthesia.Deform
{
    public class DynamicNoiseDeformationComponent : AdditiveDeformationComponent
    {
        private delegate JobHandle JobScheduleDelegate (
            UnityEngine.Mesh.MeshData meshData, 
            NativeArray<Sample4> deformations, NativeArray<TriNoise.DomainNoiseComponent> configs, 
            int innerloopBatchCount, JobHandle dependency
        );

        private delegate Sample4 ComputeDelegate (
            float3x4 v, 
            NativeArray<TriNoise.DomainNoiseComponent> configs
        );
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
        private struct Job<N> : IJobFor where N : struct, ISimpleNoise, INoise
        {
            [ReadOnly] private NativeArray<Vertex4> vertices;
            [NativeDisableContainerSafetyRestriction][ReadOnly] private NativeArray<TriNoise.DomainNoiseComponent> configs;
            [NativeDisableContainerSafetyRestriction][WriteOnly] private NativeArray<Sample4> deformations;

            public void Execute(int i)
            {
                Vertex4 v = vertices[i];
                float4 noise = default;
                for (int c = 0; c < configs.Length; c++)
                {
                    TriNoise.DomainNoiseComponent component = configs[c];
                    float4x3 position = component.DomainTRS.TransformVectors(transpose(float3x4(
                        v.v0.position, v.v1.position, v.v2.position, v.v3.position
                    )));
                    noise += position.GetSimpleNoise<N>(component.Component);
                }
                deformations[i] = noise;
            }

            /// <summary>
            /// Used for single on the fly computes 
            /// </summary>
            /// <param name="v"></param>
            /// <param name="configs"></param>
            /// <returns></returns>
            public static Sample4 Compute(float3x4 v, NativeArray<TriNoise.DomainNoiseComponent> configs)
            {
                Sample4 noise = default;
                for (int c = 0; c < configs.Length; c++)
                {
                    TriNoise.DomainNoiseComponent component = configs[c];
                    float4x3 position = component.DomainTRS.TransformVectors(transpose(v));
                    noise += position.GetNoise<N>(component.Component, component.DerivativeMatrix);
                }

                return noise;
            }
        
            public static JobHandle ScheduleParallel (UnityEngine.Mesh.MeshData meshData, 
                NativeArray<Sample4> deformations, NativeArray<TriNoise.DomainNoiseComponent> configs, 
                int innerloopBatchCount, JobHandle dependency)
            {
                return new Job<N>
                {
                    vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
                    deformations = deformations,
                    configs = configs,
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

        private static readonly ComputeDelegate[,] _computes = {
            {
                Job<Lattice1D<Perlin, LatticeNormal>>.Compute,
                Job<Lattice2D<Perlin, LatticeNormal>>.Compute,
                Job<Lattice3D<Perlin, LatticeNormal>>.Compute
            },
            {
                Job<Lattice1D<Smoothstep<Turbulence<Perlin>>, LatticeNormal>>.Compute,
                Job<Lattice2D<Smoothstep<Turbulence<Perlin>>, LatticeNormal>>.Compute,
                Job<Lattice3D<Smoothstep<Turbulence<Perlin>>, LatticeNormal>>.Compute
            },
            {
                Job<Lattice1D<Value, LatticeNormal>>.Compute,
                Job<Lattice2D<Value, LatticeNormal>>.Compute,
                Job<Lattice3D<Value, LatticeNormal>>.Compute
            },
            {
                Job<Simplex1D<Simplex>>.Compute,
                Job<Simplex2D<Simplex>>.Compute,
                Job<Simplex3D<Simplex>>.Compute
            },
            {
                Job<Simplex1D<Turbulence<Simplex>>>.Compute,
                Job<Simplex2D<Turbulence<Simplex>>>.Compute,
                Job<Simplex3D<Turbulence<Simplex>>>.Compute
            },
            {
                Job<Simplex1D<Smoothstep<Turbulence<Simplex>>>>.Compute,
                Job<Simplex2D<Smoothstep<Turbulence<Simplex>>>>.Compute,
                Job<Simplex3D<Smoothstep<Turbulence<Simplex>>>>.Compute
            },
            {
                Job<Simplex1D<Value>>.Compute,
                Job<Simplex2D<Value>>.Compute,
                Job<Simplex3D<Value>>.Compute
            },
            {
                Job<Voronoi1D<LatticeNormal, Worley, F1>>.Compute,
                Job<Voronoi2D<LatticeNormal, Worley, F1>>.Compute,
                Job<Voronoi3D<LatticeNormal, Worley, F1>>.Compute
            },
            {
                Job<Voronoi1D<LatticeNormal, Worley, F2>>.Compute,
                Job<Voronoi2D<LatticeNormal, Worley, F2>>.Compute,
                Job<Voronoi3D<LatticeNormal, Worley, F2>>.Compute
            },
            {
                Job<Voronoi1D<LatticeNormal, Worley, F2MinusF1>>.Compute,
                Job<Voronoi2D<LatticeNormal, Worley, F2MinusF1>>.Compute,
                Job<Voronoi3D<LatticeNormal, Worley, F2MinusF1>>.Compute
            },
            {
                Job<Voronoi1D<LatticeNormal, SmoothWorley, F1>>.Compute,
                Job<Voronoi2D<LatticeNormal, SmoothWorley, F1>>.Compute,
                Job<Voronoi3D<LatticeNormal, SmoothWorley, F1>>.Compute
            },
            {
                Job<Voronoi1D<LatticeNormal, SmoothWorley, F2>>.Compute,
                Job<Voronoi2D<LatticeNormal, SmoothWorley, F2>>.Compute,
                Job<Voronoi3D<LatticeNormal, SmoothWorley, F2>>.Compute
            },
            {
                Job<Voronoi1D<LatticeNormal, Chebyshev, F1>>.Compute,
                Job<Voronoi2D<LatticeNormal, Chebyshev, F1>>.Compute,
                Job<Voronoi3D<LatticeNormal, Chebyshev, F1>>.Compute
            },
            {
                Job<Voronoi1D<LatticeNormal, Chebyshev, F2>>.Compute,
                Job<Voronoi2D<LatticeNormal, Chebyshev, F2>>.Compute,
                Job<Voronoi3D<LatticeNormal, Chebyshev, F2>>.Compute
            },
            {
                Job<Voronoi1D<LatticeNormal, Chebyshev, F2MinusF1>>.Compute,
                Job<Voronoi2D<LatticeNormal, Chebyshev, F2MinusF1>>.Compute,
                Job<Voronoi3D<LatticeNormal, Chebyshev, F2MinusF1>>.Compute
            }
        };

        [SerializeField] private CatlikeNoiseType _noiseType;
        
        [SerializeField, Range(1, 3)] private int _dimensions = 3;

        [SerializeField] private DynamicNoiseConfiguration _configuration;

        public override JobHandle MeshDeformation(UnityEngine.Mesh.MeshData meshData, NativeArray<Sample4> deformations, int innerloopBatchCount, JobHandle dependency)
        {
            return _jobs[(int) _noiseType, _dimensions - 1](
                meshData,
                deformations,
                _configuration.NoiseComponents,
                innerloopBatchCount,
                dependency);
        }

        public override Sample4 VertexDeformation(float3x4 vertex)
        {
            return _computes[(int) _noiseType, _dimensions - 1](vertex, _configuration.NoiseComponents);
        }
    }
}