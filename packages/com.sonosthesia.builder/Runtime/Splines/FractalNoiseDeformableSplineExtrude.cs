using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Builder
{
    public class FractalNoiseDeformableSplineExtrude : NoiseDeformableSplineExtrude
    {
        [SerializeField] private Noise.Settings _noiseSettings = Noise.Settings.Default;
        
        [SerializeField] private SpaceTRS _domain = new SpaceTRS { scale = 1f };

        [SerializeField] private float _displacement = 0.1f;
        
        private delegate JobHandle JobScheduleDelegate (
            Mesh.MeshData meshData, int innerloopBatchCount, Noise.Settings settings, int seed, SpaceTRS domain, 
            float displacement, JobHandle dependency
        );
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        public struct Job<N> : IJobFor where N : struct, Noise.INoise
        {
            private Noise.Settings settings;
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
        
            public static JobHandle ScheduleParallel (Mesh.MeshData meshData, int innerloopBatchCount, 
                Noise.Settings settings, int seed, SpaceTRS domain, float displacement, 
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
        
        protected override void Deform(ISpline spline, Mesh.MeshData data, 
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