using Sonosthesia.Mesh;
using Sonosthesia.Noise;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Deform
{
    public class FractalNoiseDeformableSplineExtrude : NoiseDeformableSplineExtrude
    {
        [SerializeField] private FractalNoiseSettings _noiseSettings = FractalNoiseSettings.Default;
        
        [SerializeField] private SpaceTRS _domain = new SpaceTRS { scale = 1f };

        [SerializeField] private float _displacement = 0.1f;
        
        private delegate JobHandle JobScheduleDelegate (
            UnityEngine.Mesh.MeshData meshData, int innerloopBatchCount, FractalNoiseSettings settings, int seed, SpaceTRS domain, 
            float displacement, JobHandle dependency
        );
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        public struct Job<N> : IJobFor where N : struct, INoise
        {
            private FractalNoiseSettings settings;
            private int seed;
            private float3x4 domainTRS;
            private float3x3 derivativeMatrix;
            private float displacement;
            private NativeArray<SplineVertexData4> vertices;

            public void Execute(int i)
            {
                SplineVertexData4 v = vertices[i];
                Sample4 noise = SplineUtils.GetFractalNoise<N>(v, domainTRS, settings, seed) * displacement;
                noise.Derivatives = derivativeMatrix.TransformVectors(noise.Derivatives);
                vertices[i] = SplineUtils.DeformVerticesAlongNormals(v, noise);
            }
        
            public static JobHandle ScheduleParallel (UnityEngine.Mesh.MeshData meshData, int innerloopBatchCount, 
                FractalNoiseSettings settings, int seed, SpaceTRS domain, float displacement, 
                JobHandle dependency
            ) => 
                new Job<N>
                {
                    vertices = meshData.GetVertexData<SplineVertexData>().Reinterpret<SplineVertexData4>(12 + 12 + 8),
                    settings = settings,
                    seed = seed,
                    domainTRS = domain.Matrix,
                    derivativeMatrix = domain.DerivativeMatrix,
                    displacement = displacement
                }.ScheduleParallel(meshData.vertexCount / 4, innerloopBatchCount, dependency);
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
        
        protected override void Deform(ISpline spline, UnityEngine.Mesh.MeshData data, 
            float radius, int sides, float segmentsPerUnit, bool capped, float2 range,
            NoiseType noiseType, int dimensions, int seed)
        {
            int innerloopBatchCount = (int)math.sqrt(segmentsPerUnit);
            
            _jobs[(int) noiseType, dimensions - 1](
                data,
                innerloopBatchCount,
                _noiseSettings,
                seed,
                _domain,
                _displacement,
                default).Complete();
        }
    }
}