using Unity.Jobs;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class MonoMeshNoiseController : CatlikeMeshNoiseController
    {
        private static SurfaceJobScheduleDelegate[,] _jobs = {
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
        
        protected override JobHandle PerturbMesh(
            Mesh.MeshData meshData, int resolution, float displacement, 
            NoiseType noiseType, int dimensions, Noise.Settings settings, int seed, SpaceTRS domain,
            JobHandle dependency)
        {
            return _jobs[(int) noiseType, dimensions - 1](
                meshData,
                resolution,
                settings,
                seed,
                domain,
                displacement,
                IsPlane,
                dependency);
        }
        
    }
}