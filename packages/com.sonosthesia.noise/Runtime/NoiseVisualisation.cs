using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Noise
{
    public class NoiseVisualisation : Visualisation
    {
        private static FractalNoise.ScheduleDelegate[,] _noiseJobs = {
            {
                FractalNoise.Job<Lattice1D<Perlin, LatticeNormal>>.ScheduleParallel,
                FractalNoise.Job<Lattice1D<Perlin, LatticeTiling>>.ScheduleParallel,
                FractalNoise.Job<Lattice2D<Perlin, LatticeNormal>>.ScheduleParallel,
                FractalNoise.Job<Lattice2D<Perlin, LatticeTiling>>.ScheduleParallel,
                FractalNoise.Job<Lattice3D<Perlin, LatticeNormal>>.ScheduleParallel ,
                FractalNoise.Job<Lattice3D<Perlin, LatticeTiling>>.ScheduleParallel
            },
            {
                FractalNoise.Job<Lattice1D<Turbulence<Perlin>, LatticeNormal>>.ScheduleParallel,
                FractalNoise.Job<Lattice1D<Turbulence<Perlin>, LatticeTiling>>.ScheduleParallel,
                FractalNoise.Job<Lattice2D<Turbulence<Perlin>, LatticeNormal>>.ScheduleParallel,
                FractalNoise.Job<Lattice2D<Turbulence<Perlin>, LatticeTiling>>.ScheduleParallel,
                FractalNoise.Job<Lattice3D<Turbulence<Perlin>, LatticeNormal>>.ScheduleParallel,
                FractalNoise.Job<Lattice3D<Turbulence<Perlin>, LatticeTiling>>.ScheduleParallel
            },
            {
                FractalNoise.Job<Lattice1D<Value, LatticeNormal>>.ScheduleParallel,
                FractalNoise.Job<Lattice1D<Value, LatticeTiling>>.ScheduleParallel,
                FractalNoise.Job<Lattice2D<Value, LatticeNormal>>.ScheduleParallel,
                FractalNoise.Job<Lattice2D<Value, LatticeTiling>>.ScheduleParallel,
                FractalNoise.Job<Lattice3D<Value, LatticeNormal>>.ScheduleParallel,
                FractalNoise.Job<Lattice3D<Value, LatticeTiling>>.ScheduleParallel
            },
            {
                FractalNoise.Job<Lattice1D<Turbulence<Value>, LatticeNormal>>.ScheduleParallel,
                FractalNoise.Job<Lattice1D<Turbulence<Value>, LatticeTiling>>.ScheduleParallel,
                FractalNoise.Job<Lattice2D<Turbulence<Value>, LatticeNormal>>.ScheduleParallel,
                FractalNoise.Job<Lattice2D<Turbulence<Value>, LatticeTiling>>.ScheduleParallel,
                FractalNoise.Job<Lattice3D<Turbulence<Value>, LatticeNormal>>.ScheduleParallel,
                FractalNoise.Job<Lattice3D<Turbulence<Value>, LatticeTiling>>.ScheduleParallel
            },
            {
                FractalNoise.Job<Simplex1D<Simplex>>.ScheduleParallel,
                FractalNoise.Job<Simplex1D<Simplex>>.ScheduleParallel,
                FractalNoise.Job<Simplex2D<Simplex>>.ScheduleParallel,
                FractalNoise.Job<Simplex2D<Simplex>>.ScheduleParallel,
                FractalNoise.Job<Simplex3D<Simplex>>.ScheduleParallel,
                FractalNoise.Job<Simplex3D<Simplex>>.ScheduleParallel
            },
            {
                FractalNoise.Job<Simplex1D<Turbulence<Simplex>>>.ScheduleParallel,
                FractalNoise.Job<Simplex1D<Turbulence<Simplex>>>.ScheduleParallel,
                FractalNoise.Job<Simplex2D<Turbulence<Simplex>>>.ScheduleParallel,
                FractalNoise.Job<Simplex2D<Turbulence<Simplex>>>.ScheduleParallel,
                FractalNoise.Job<Simplex3D<Turbulence<Simplex>>>.ScheduleParallel,
                FractalNoise.Job<Simplex3D<Turbulence<Simplex>>>.ScheduleParallel
            },
            {
                FractalNoise.Job<Simplex1D<Value>>.ScheduleParallel,
                FractalNoise.Job<Simplex1D<Value>>.ScheduleParallel,
                FractalNoise.Job<Simplex2D<Value>>.ScheduleParallel,
                FractalNoise.Job<Simplex2D<Value>>.ScheduleParallel,
                FractalNoise.Job<Simplex3D<Value>>.ScheduleParallel,
                FractalNoise.Job<Simplex3D<Value>>.ScheduleParallel
            },
            {
                FractalNoise.Job<Simplex1D<Turbulence<Value>>>.ScheduleParallel,
                FractalNoise.Job<Simplex1D<Turbulence<Value>>>.ScheduleParallel,
                FractalNoise.Job<Simplex2D<Turbulence<Value>>>.ScheduleParallel,
                FractalNoise.Job<Simplex2D<Turbulence<Value>>>.ScheduleParallel,
                FractalNoise.Job<Simplex3D<Turbulence<Value>>>.ScheduleParallel,
                FractalNoise.Job<Simplex3D<Turbulence<Value>>>.ScheduleParallel
            },
            {
                FractalNoise.Job<Voronoi1D<LatticeNormal, Worley, F1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi1D<LatticeTiling, Worley, F1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi2D<LatticeNormal, Worley, F1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi2D<LatticeTiling, Worley, F1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi3D<LatticeNormal, Worley, F1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi3D<LatticeTiling, Worley, F1>>.ScheduleParallel
            },
            {
                FractalNoise.Job<Voronoi1D<LatticeNormal, Worley, F2>>.ScheduleParallel,
                FractalNoise.Job<Voronoi1D<LatticeTiling, Worley, F2>>.ScheduleParallel,
                FractalNoise.Job<Voronoi2D<LatticeNormal, Worley, F2>>.ScheduleParallel,
                FractalNoise.Job<Voronoi2D<LatticeTiling, Worley, F2>>.ScheduleParallel,
                FractalNoise.Job<Voronoi3D<LatticeNormal, Worley, F2>>.ScheduleParallel,
                FractalNoise.Job<Voronoi3D<LatticeTiling, Worley, F2>>.ScheduleParallel
            },
            {
                FractalNoise.Job<Voronoi1D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi1D<LatticeTiling, Worley, F2MinusF1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi2D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi2D<LatticeTiling, Worley, F2MinusF1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi3D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi3D<LatticeTiling, Worley, F2MinusF1>>.ScheduleParallel
            },
            {
                FractalNoise.Job<Voronoi1D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi1D<LatticeTiling, Chebyshev, F1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi2D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi2D<LatticeTiling, Chebyshev, F1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi3D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi3D<LatticeTiling, Chebyshev, F1>>.ScheduleParallel
            },
            {
                FractalNoise.Job<Voronoi1D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel,
                FractalNoise.Job<Voronoi1D<LatticeTiling, Chebyshev, F2>>.ScheduleParallel,
                FractalNoise.Job<Voronoi2D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel,
                FractalNoise.Job<Voronoi2D<LatticeTiling, Chebyshev, F2>>.ScheduleParallel,
                FractalNoise.Job<Voronoi3D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel,
                FractalNoise.Job<Voronoi3D<LatticeTiling, Chebyshev, F2>>.ScheduleParallel
            },
            {
                FractalNoise.Job<Voronoi1D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi1D<LatticeTiling, Chebyshev, F2MinusF1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi2D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi2D<LatticeTiling, Chebyshev, F2MinusF1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi3D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel,
                FractalNoise.Job<Voronoi3D<LatticeTiling, Chebyshev, F2MinusF1>>.ScheduleParallel
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
        
        [SerializeField] private FractalNoiseSettings _settings = FractalNoiseSettings.Default;

        [SerializeField] private int _seed;

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
            _noiseJobs[(int)_type, 2 * _dimensions - (_tiling ? 1 : 2)](positions, _noise, _settings, _seed, domain, resolution, handle).Complete();
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