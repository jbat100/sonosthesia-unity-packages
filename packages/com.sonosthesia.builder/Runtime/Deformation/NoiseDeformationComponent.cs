using Codice.CM.SEIDInfo;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace Sonosthesia.Builder
{
    public class NoiseDeformationComponent : DeformationComponent
    {
        [SerializeField] private bool _tiling;
        
        [SerializeField] private NoiseType _type;
	
        [SerializeField, Range(1, 3)] int _dimensions = 3;
        
        [SerializeField] private Noise.Settings _settings = Noise.Settings.Default;

        [SerializeField] private float _displacement = 1f;
        
        static SurfaceJobScheduleDelegate[,] _surfaceJobs = {
            {
                SurfaceJob<Noise.Lattice1D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Noise.Lattice2D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Noise.Lattice3D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Lattice1D<Noise.Smoothstep<Noise.Turbulence<Noise.Perlin>>, Noise.LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Noise.Lattice2D<Noise.Smoothstep<Noise.Turbulence<Noise.Perlin>>, Noise.LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Noise.Lattice3D<Noise.Smoothstep<Noise.Turbulence<Noise.Perlin>>, Noise.LatticeNormal>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Lattice1D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Noise.Lattice2D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Noise.Lattice3D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Simplex1D<Noise.Simplex>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex2D<Noise.Simplex>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex3D<Noise.Simplex>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Simplex1D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex2D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex3D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Simplex1D<Noise.Smoothstep<Noise.Turbulence<Noise.Simplex>>>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex2D<Noise.Smoothstep<Noise.Turbulence<Noise.Simplex>>>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex3D<Noise.Smoothstep<Noise.Turbulence<Noise.Simplex>>>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Simplex1D<Noise.Value>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex2D<Noise.Value>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex3D<Noise.Value>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F1>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F2>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F2>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F2>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel
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

        public override JobHandle GetDeformation(NativeArray<float3x4> positions, NativeArray<Sample4> deformations)
        {
            return default;
        }
    }

    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    struct NoiseDeformationJob<N> : IJobFor where N : struct, Noise.INoise
    {
        [ReadOnly] public NativeArray<float3x4> positions;

        [WriteOnly] public NativeArray<Sample4> deformation;

        public Noise.Settings settings;

        public float3x4 domainTRS;

        public float time;

        public float displacement;
        
        public void Execute (int i)
        {
            deformation[i] = Noise.GetFractalNoise<N>(
                domainTRS.TransformVectors(transpose(positions[i])), settings
            ).v;
        }

        public static JobHandle ScheduleParallel (
            NativeArray<float3x4> positions, NativeArray<Sample4> deformation,
            Noise.Settings settings, SpaceTRS domainTRS, int resolution, float time, JobHandle dependency
        ) => new NoiseDeformationJob<N> {
            positions = positions,
            deformation = deformation,
            settings = settings,
            domainTRS = domainTRS.Matrix,
            time = time
        }.ScheduleParallel(positions.Length, resolution, dependency);

    }
    
}