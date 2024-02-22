using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Sonosthesia.Mesh;
using Sonosthesia.Noise;

using static Unity.Mathematics.math;

namespace Sonosthesia.Deform
{
    public class LerpNoiseDeformationComponent : AdditiveDeformationComponent
    {
        [SerializeField] private NoiseType _noiseType;

        [SerializeField] private int _seedOffset;
	
        [SerializeField, Range(1, 3)] int _dimensions = 3;
        
        [SerializeField] private FractalNoiseSettings _settings = FractalNoiseSettings.Default;

        [SerializeField] private float _displacement = 1f;

        [SerializeField] private SpaceTRS _domain;
        
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct Job<N> : IJobFor where N : struct, INoise
        {
            [ReadOnly] private NativeArray<Vertex4> vertices;

            [WriteOnly] private NativeArray<Sample4> deformations;

            private FractalNoiseSettings settings;

            private float3x4 domainTRS;

            private int seed;
        
            private float lerp;
            
            private float displacement;

            public void Execute (int i)
            {
                Vertex4 v = vertices[i];
                float4x3 position = domainTRS.TransformVectors(transpose(float3x4(
                    v.v0.position, v.v1.position, v.v2.position, v.v3.position
                )));
                Sample4 d0 = FractalNoise.GetFractalNoise<N>(position, settings, seed);
                Sample4 d1 = FractalNoise.GetFractalNoise<N>(position, settings, seed + 1);
                deformations[i] = (d0 * (1 - lerp) + d1 * lerp) * displacement;
            }

            public static JobHandle ScheduleParallel (
                UnityEngine.Mesh.MeshData meshData, NativeArray<Sample4> deformations,
                FractalNoiseSettings settings, int seed, SpaceTRS domainTRS, float lerp, float displacement,
                int innerloopBatchCount, JobHandle dependency
            ) => new Job<N> {
                vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
                deformations = deformations,
                settings = settings,
                domainTRS = domainTRS.Matrix,
                seed = seed,
                lerp = lerp,
                displacement = displacement
            }.ScheduleParallel(meshData.vertexCount / 4, innerloopBatchCount, dependency);

        }
    
        private delegate JobHandle JobScheduleDelegate (
            UnityEngine.Mesh.MeshData meshData, NativeArray<Sample4> deformations,
            FractalNoiseSettings settings, int seed, SpaceTRS domainTRS, float lerp, float displacement, 
            int innerloopBatchCount, JobHandle dependency
        );

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

        public enum NoiseType 
        {
            Perlin, PerlinSmoothTurbulence, PerlinValue, 
            Simplex, SimplexTurbulence, SimplexSmoothTurbulence, SimplexValue,
            VoronoiWorleyF1, VoronoiWorleyF2, VoronoiWorleyF2MinusF1, 
            VoronoiWorleySmoothLSE, VoronoiWorleySmoothPoly,
            VoronoiChebyshevF1, VoronoiChebyshevF2, VoronoiChebyshevF2MinusF1
        }
        
        public override bool IsDynamic => true;
        
        public override JobHandle MeshDeformation(UnityEngine.Mesh.MeshData meshData, 
            NativeArray<Sample4> deformations, int innerloopBatchCount, JobHandle dependency)
        {
            float time = Time.time;
            int seed = Mathf.FloorToInt(time);
            float lerp = time - seed;

            return _jobs[(int) _noiseType, _dimensions - 1](
                meshData,
                deformations,
                _settings,
                seed + _seedOffset,
                _domain,
                lerp,
                _displacement,
                innerloopBatchCount, 
                dependency);
        }

        public override Sample4 VertexDeformation(float3x4 vertex)
        {
            throw new System.NotImplementedException();
        }
    }
}