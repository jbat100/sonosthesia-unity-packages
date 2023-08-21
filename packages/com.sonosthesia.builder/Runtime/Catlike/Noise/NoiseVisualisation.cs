using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class NoiseVisualisation : Visualisation
    {
        private static Noise.ScheduleDelegate[,] _noiseJobs = {
            {
                Noise.Job<Noise.Lattice1D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel,
                Noise.Job<Noise.Lattice1D<Noise.Perlin, Noise.LatticeTiling>>.ScheduleParallel,
                Noise.Job<Noise.Lattice2D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel,
                Noise.Job<Noise.Lattice2D<Noise.Perlin, Noise.LatticeTiling>>.ScheduleParallel,
                Noise.Job<Noise.Lattice3D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel ,
                Noise.Job<Noise.Lattice3D<Noise.Perlin, Noise.LatticeTiling>>.ScheduleParallel
            },
            {
                Noise.Job<Noise.Lattice1D<Noise.Turbulence<Noise.Perlin>, Noise.LatticeNormal>>.ScheduleParallel,
                Noise.Job<Noise.Lattice1D<Noise.Turbulence<Noise.Perlin>, Noise.LatticeTiling>>.ScheduleParallel,
                Noise.Job<Noise.Lattice2D<Noise.Turbulence<Noise.Perlin>, Noise.LatticeNormal>>.ScheduleParallel,
                Noise.Job<Noise.Lattice2D<Noise.Turbulence<Noise.Perlin>, Noise.LatticeTiling>>.ScheduleParallel,
                Noise.Job<Noise.Lattice3D<Noise.Turbulence<Noise.Perlin>, Noise.LatticeNormal>>.ScheduleParallel,
                Noise.Job<Noise.Lattice3D<Noise.Turbulence<Noise.Perlin>, Noise.LatticeTiling>>.ScheduleParallel
            },
            {
                Noise.Job<Noise.Lattice1D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel,
                Noise.Job<Noise.Lattice1D<Noise.Value, Noise.LatticeTiling>>.ScheduleParallel,
                Noise.Job<Noise.Lattice2D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel,
                Noise.Job<Noise.Lattice2D<Noise.Value, Noise.LatticeTiling>>.ScheduleParallel,
                Noise.Job<Noise.Lattice3D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel,
                Noise.Job<Noise.Lattice3D<Noise.Value, Noise.LatticeTiling>>.ScheduleParallel
            },
            {
                Noise.Job<Noise.Lattice1D<Noise.Turbulence<Noise.Value>, Noise.LatticeNormal>>.ScheduleParallel,
                Noise.Job<Noise.Lattice1D<Noise.Turbulence<Noise.Value>, Noise.LatticeTiling>>.ScheduleParallel,
                Noise.Job<Noise.Lattice2D<Noise.Turbulence<Noise.Value>, Noise.LatticeNormal>>.ScheduleParallel,
                Noise.Job<Noise.Lattice2D<Noise.Turbulence<Noise.Value>, Noise.LatticeTiling>>.ScheduleParallel,
                Noise.Job<Noise.Lattice3D<Noise.Turbulence<Noise.Value>, Noise.LatticeNormal>>.ScheduleParallel,
                Noise.Job<Noise.Lattice3D<Noise.Turbulence<Noise.Value>, Noise.LatticeTiling>>.ScheduleParallel
            },
            {
                Noise.Job<Noise.Simplex1D<Noise.Simplex>>.ScheduleParallel,
                Noise.Job<Noise.Simplex1D<Noise.Simplex>>.ScheduleParallel,
                Noise.Job<Noise.Simplex2D<Noise.Simplex>>.ScheduleParallel,
                Noise.Job<Noise.Simplex2D<Noise.Simplex>>.ScheduleParallel,
                Noise.Job<Noise.Simplex3D<Noise.Simplex>>.ScheduleParallel,
                Noise.Job<Noise.Simplex3D<Noise.Simplex>>.ScheduleParallel
            },
            {
                Noise.Job<Noise.Simplex1D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel,
                Noise.Job<Noise.Simplex1D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel,
                Noise.Job<Noise.Simplex2D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel,
                Noise.Job<Noise.Simplex2D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel,
                Noise.Job<Noise.Simplex3D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel,
                Noise.Job<Noise.Simplex3D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel
            },
            {
                Noise.Job<Noise.Simplex1D<Noise.Value>>.ScheduleParallel,
                Noise.Job<Noise.Simplex1D<Noise.Value>>.ScheduleParallel,
                Noise.Job<Noise.Simplex2D<Noise.Value>>.ScheduleParallel,
                Noise.Job<Noise.Simplex2D<Noise.Value>>.ScheduleParallel,
                Noise.Job<Noise.Simplex3D<Noise.Value>>.ScheduleParallel,
                Noise.Job<Noise.Simplex3D<Noise.Value>>.ScheduleParallel
            },
            {
                Noise.Job<Noise.Simplex1D<Noise.Turbulence<Noise.Value>>>.ScheduleParallel,
                Noise.Job<Noise.Simplex1D<Noise.Turbulence<Noise.Value>>>.ScheduleParallel,
                Noise.Job<Noise.Simplex2D<Noise.Turbulence<Noise.Value>>>.ScheduleParallel,
                Noise.Job<Noise.Simplex2D<Noise.Turbulence<Noise.Value>>>.ScheduleParallel,
                Noise.Job<Noise.Simplex3D<Noise.Turbulence<Noise.Value>>>.ScheduleParallel,
                Noise.Job<Noise.Simplex3D<Noise.Turbulence<Noise.Value>>>.ScheduleParallel
            },
            {
                Noise.Job<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi1D<Noise.LatticeTiling, Noise.Worley, Noise.F1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi2D<Noise.LatticeTiling, Noise.Worley, Noise.F1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi3D<Noise.LatticeTiling, Noise.Worley, Noise.F1>>.ScheduleParallel
            },
            {
                Noise.Job<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi1D<Noise.LatticeTiling, Noise.Worley, Noise.F2>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi2D<Noise.LatticeTiling, Noise.Worley, Noise.F2>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi3D<Noise.LatticeTiling, Noise.Worley, Noise.F2>>.ScheduleParallel
            },
            {
                Noise.Job<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi1D<Noise.LatticeTiling, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi2D<Noise.LatticeTiling, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi3D<Noise.LatticeTiling, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel
            },
            {
                Noise.Job<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi1D<Noise.LatticeTiling, Noise.Chebyshev, Noise.F1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi2D<Noise.LatticeTiling, Noise.Chebyshev, Noise.F1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi3D<Noise.LatticeTiling, Noise.Chebyshev, Noise.F1>>.ScheduleParallel
            },
            {
                Noise.Job<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi1D<Noise.LatticeTiling, Noise.Chebyshev, Noise.F2>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi2D<Noise.LatticeTiling, Noise.Chebyshev, Noise.F2>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi3D<Noise.LatticeTiling, Noise.Chebyshev, Noise.F2>>.ScheduleParallel
            },
            {
                Noise.Job<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi1D<Noise.LatticeTiling, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi2D<Noise.LatticeTiling, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel,
                Noise.Job<Noise.Voronoi3D<Noise.LatticeTiling, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel
            }
        };
        
        private static int _noiseId = Shader.PropertyToID("_Noise");

        public enum NoiseType
        {
            Perlin, 
            PerlinTurbulence,
            Value,
            ValueTurbulence,
            Simplex, 
            SimplexTurbulence, 
            SimplexValue, 
            SimplexValueTurbulence,
            VoronoiWorleyF1,
            VoronoiWorleyF2,
            VoronoiWorleyF2MinusF1,
            VoronoiChebyshevF1,
            VoronoiChebyshevF2,
            VoronoiChebyshevF2MinusF1
        }

        [SerializeField] private bool _tiling;
        
        [SerializeField] private NoiseType _type;
	
        [SerializeField, Range(1, 3)] int _dimensions = 3;
        
        [SerializeField] private Noise.Settings _settings = Noise.Settings.Default;

        [SerializeField] private SpaceTRS domain = new SpaceTRS { scale = 8f };

        private NativeArray<float4> _noise;

        private ComputeBuffer _noiseBuffer;
        
        protected override void EnableVisualization(int dataLength, MaterialPropertyBlock propertyBlock)
        {
            _noise = new NativeArray<float4>(dataLength, Allocator.Persistent);
            _noiseBuffer = new ComputeBuffer(dataLength * 4, 4);
            propertyBlock.SetBuffer(_noiseId, _noiseBuffer);
        }

        protected override void UpdateVisualization(NativeArray<float3x4> positions, int resolution, JobHandle handle)
        {
            _noiseJobs[(int)_type, 2 * _dimensions - (_tiling ? 1 : 2)](positions, _noise, _settings, domain, resolution, handle).Complete();
            _noiseBuffer.SetData(_noise.Reinterpret<float>(4 * 4));
        }

        protected override void DisableVisualization()
        {
            _noise.Dispose();
            _noiseBuffer.Release();
            _noiseBuffer = null;
        }
    }
}