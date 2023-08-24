using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class NoiseDeformationComponent : DeformationComponent
    {
        [SerializeField] private bool _tiling;
        
        [SerializeField] private NoiseType _noiseType;

        [SerializeField] private int _seedOffset;
	
        [SerializeField, Range(1, 3)] int _dimensions = 3;
        
        [SerializeField] private Noise.Settings _settings = Noise.Settings.Default;

        [SerializeField] private float _displacement = 1f;
        
        static PositionNoiseDeformationJobScheduleDelegate[,] _positionDeformationJobs = {
            {
                PositionNoiseDeformationJob<Noise.Lattice1D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Lattice2D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Lattice3D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel
            },
            {
                PositionNoiseDeformationJob<Noise.Lattice1D<Noise.Smoothstep<Noise.Turbulence<Noise.Perlin>>, Noise.LatticeNormal>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Lattice2D<Noise.Smoothstep<Noise.Turbulence<Noise.Perlin>>, Noise.LatticeNormal>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Lattice3D<Noise.Smoothstep<Noise.Turbulence<Noise.Perlin>>, Noise.LatticeNormal>>.ScheduleParallel
            },
            {
                PositionNoiseDeformationJob<Noise.Lattice1D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Lattice2D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Lattice3D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel
            },
            {
                PositionNoiseDeformationJob<Noise.Simplex1D<Noise.Simplex>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Simplex2D<Noise.Simplex>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Simplex3D<Noise.Simplex>>.ScheduleParallel
            },
            {
                PositionNoiseDeformationJob<Noise.Simplex1D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Simplex2D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Simplex3D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel
            },
            {
                PositionNoiseDeformationJob<Noise.Simplex1D<Noise.Smoothstep<Noise.Turbulence<Noise.Simplex>>>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Simplex2D<Noise.Smoothstep<Noise.Turbulence<Noise.Simplex>>>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Simplex3D<Noise.Smoothstep<Noise.Turbulence<Noise.Simplex>>>>.ScheduleParallel
            },
            {
                PositionNoiseDeformationJob<Noise.Simplex1D<Noise.Value>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Simplex2D<Noise.Value>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Simplex3D<Noise.Value>>.ScheduleParallel
            },
            {
                PositionNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel
            },
            {
                PositionNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel
            },
            {
                PositionNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel
            },
            {
                PositionNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F1>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F1>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F1>>.ScheduleParallel
            },
            {
                PositionNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F2>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F2>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F2>>.ScheduleParallel
            },
            {
                PositionNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel
            },
            {
                PositionNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel
            },
            {
                PositionNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel,
                PositionNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel
            }
        };
        
        
        static VertexNoiseDeformationJobScheduleDelegate[,] _vertexDeformationJobs = {
            {
                VertexNoiseDeformationJob<Noise.Lattice1D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Lattice2D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Lattice3D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel
            },
            {
                VertexNoiseDeformationJob<Noise.Lattice1D<Noise.Smoothstep<Noise.Turbulence<Noise.Perlin>>, Noise.LatticeNormal>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Lattice2D<Noise.Smoothstep<Noise.Turbulence<Noise.Perlin>>, Noise.LatticeNormal>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Lattice3D<Noise.Smoothstep<Noise.Turbulence<Noise.Perlin>>, Noise.LatticeNormal>>.ScheduleParallel
            },
            {
                VertexNoiseDeformationJob<Noise.Lattice1D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Lattice2D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Lattice3D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel
            },
            {
                VertexNoiseDeformationJob<Noise.Simplex1D<Noise.Simplex>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Simplex2D<Noise.Simplex>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Simplex3D<Noise.Simplex>>.ScheduleParallel
            },
            {
                VertexNoiseDeformationJob<Noise.Simplex1D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Simplex2D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Simplex3D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel
            },
            {
                VertexNoiseDeformationJob<Noise.Simplex1D<Noise.Smoothstep<Noise.Turbulence<Noise.Simplex>>>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Simplex2D<Noise.Smoothstep<Noise.Turbulence<Noise.Simplex>>>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Simplex3D<Noise.Smoothstep<Noise.Turbulence<Noise.Simplex>>>>.ScheduleParallel
            },
            {
                VertexNoiseDeformationJob<Noise.Simplex1D<Noise.Value>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Simplex2D<Noise.Value>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Simplex3D<Noise.Value>>.ScheduleParallel
            },
            {
                VertexNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel
            },
            {
                VertexNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel
            },
            {
                VertexNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel
            },
            {
                VertexNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F1>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F1>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F1>>.ScheduleParallel
            },
            {
                VertexNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F2>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F2>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F2>>.ScheduleParallel
            },
            {
                VertexNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel
            },
            {
                VertexNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel
            },
            {
                VertexNoiseDeformationJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel,
                VertexNoiseDeformationJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel
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

        public override JobHandle GetDeformation(NativeArray<float3x4> positions, 
            NativeArray<Sample4> deformations, SpaceTRS domain, float time, int innerloopBatchCount, JobHandle dependency)
        {
            int seed = Mathf.FloorToInt(time);
            float lerp = time - seed;

            return _positionDeformationJobs[(int) _noiseType, _dimensions - 1](
                positions,
                deformations,
                _settings,
                seed + _seedOffset,
                domain,
                innerloopBatchCount,
                lerp,
                dependency);
        }
        
        public override JobHandle GetDeformation(Mesh.MeshData meshData, 
            NativeArray<Sample4> deformations, SpaceTRS domain, float time, int innerloopBatchCount, JobHandle dependency)
        {
            int seed = Mathf.FloorToInt(time);
            float lerp = time - seed;

            return _vertexDeformationJobs[(int) _noiseType, _dimensions - 1](
                meshData,
                deformations,
                _settings,
                seed + _seedOffset,
                domain,
                innerloopBatchCount,
                lerp,
                dependency);
        }
    }
}