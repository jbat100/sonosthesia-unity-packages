using Sonosthesia.Mesh;
using Sonosthesia.Noise;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace Sonosthesia.Deform
{
    public class DynamicNoisePathProcessor : PathProcessor
    {
        protected enum NoiseType 
        {
            Perlin, PerlinSmoothTurbulence, PerlinValue, 
            Simplex, SimplexTurbulence, SimplexSmoothTurbulence, SimplexValue,
            VoronoiWorleyF1, VoronoiWorleyF2, VoronoiWorleyF2MinusF1, 
            VoronoiWorleySmoothLSE, VoronoiWorleySmoothPoly,
            VoronoiChebyshevF1, VoronoiChebyshevF2, VoronoiChebyshevF2MinusF1
        }
        
        [SerializeField] private NoiseType _noiseType;

        [SerializeField, Range(1, 3)] private int _dimensions = 1;
        
        [SerializeField] private DynamicNoiseConfiguration _configuration;
        
        [SerializeField] private Vector3 _direction = Vector3.up;

        private NativeArray<TriNoise.DomainNoiseComponent> _noiseComponents;
        
        private delegate JobHandle JobScheduleDelegate (
            NativeArray<RigidTransform> points, NativeArray<TriNoise.DomainNoiseComponent> configs, float3 direction,
            int innerloopBatchCount, JobHandle dependency
        );
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct Job<N> : IJobFor where N : struct, ISimpleNoise, INoise
        {
            [NativeDisableContainerSafetyRestriction]
            private NativeArray<RigidTransform> points;
            
            [ReadOnly] 
            private NativeArray<TriNoise.DomainNoiseComponent> configs;

            private float3 direction;
            
            public void Execute(int i)
            {
                int indexOffset = i * 4;
                int maxIndex = points.Length - 1;
                
                int index0 = min(indexOffset, maxIndex);
                int index1 = min(indexOffset + 1, maxIndex);
                int index2 = min(indexOffset + 2, maxIndex);
                int index3 = min(indexOffset + 3, maxIndex);
                
                float4 noise = default;
                for (int c = 0; c < configs.Length; c++)
                {
                    TriNoise.DomainNoiseComponent component = configs[c];
                    float4x3 position = component.DomainTRS.TransformVectors(transpose(float3x4(
                        points[index0].pos, points[index1].pos, points[index2].pos, points[index3].pos
                    )));
                    noise += position.GetSimpleNoise<N>(component.Component);
                }

                RigidTransform rt0 = points[index0];
                rt0.pos += mul(rt0.rot, direction) * noise[0];
                points[index0] = rt0;
                
                RigidTransform rt1 = points[index1];
                rt1.pos += mul(rt1.rot, direction) * noise[1];
                points[index1] = rt1;
                
                RigidTransform rt2 = points[index2];
                rt2.pos += mul(rt2.rot, direction) * noise[2];
                points[index2] = rt2;
                
                RigidTransform rt3 = points[index3];
                rt3.pos += mul(rt3.rot, direction) * noise[3];
                points[index3] = rt3;
            }

            public static JobHandle ScheduleParallel (NativeArray<RigidTransform> points, 
                NativeArray<TriNoise.DomainNoiseComponent> configs, float3 direction,
                int innerloopBatchCount, JobHandle dependency)
            {
                return new Job<N>
                {
                    direction = direction,
                    points = points,
                    configs = configs,
                }.ScheduleParallel(points.Length / 4, innerloopBatchCount, dependency);
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

        public override void Process(NativeArray<RigidTransform> points)
        {
            _configuration.Populate(ref _noiseComponents);
            
            _jobs[(int) _noiseType, _dimensions - 1](
                points,
                _noiseComponents,
                _direction,
                10, 
                default).Complete();
        }
    }
}