using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class TriMeshNoiseController : CatlikeMeshNoiseController
    {
        protected override bool IsDynamic => true;

        [SerializeField] private float _velocity = 1f;

        [SerializeField] private AnimationCurve _lerpCurve;

        private readonly struct NoiseComponent
        {
            public readonly int Seed;
            public readonly float Displacement;

            public NoiseComponent(int seed, float displacement)
            {
                Seed = seed;
                Displacement = displacement;
            }

            public override string ToString()
            {
                return $"({Seed} : {Displacement})";
            }
        }
        
        private readonly struct Config
        {
            public readonly NoiseComponent C1;
            public readonly NoiseComponent C2;
            public readonly NoiseComponent C3;

            public Config(NoiseComponent c1, NoiseComponent c2, NoiseComponent c3)
            {
                C1 = c1;
                C2 = c2;
                C3 = c3;
            }

            public override string ToString()
            {
                return $"({nameof(C1)} : {C1}, {nameof(C2)} : {C2}, {nameof(C3)} : {C3})";
            }
        }
        
        private delegate JobHandle JobScheduleDelegate (
            Mesh.MeshData meshData, int resolution, Noise.Settings settings, SpaceTRS domain,
            Config config, bool isPlane, JobHandle dependency
        );
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct Job<N> : IJobFor where N : struct, Noise.INoise
        {
            private Noise.Settings settings;
            private float3x4 domainTRS;
            private float3x3 derivativeMatrix;
            private Config config;
            private bool isPlane;
            private NativeArray<Vertex4> vertices;

            public void Execute(int i)
            {
                Vertex4 v = vertices[i];
                
                Sample4 noise1 = Noise.GetFractalNoise<N>(v, domainTRS, settings, config.C1.Seed) * config.C1.Displacement;
                noise1.Derivatives = derivativeMatrix.TransformVectors(noise1.Derivatives);
                
                Sample4 noise2 = Noise.GetFractalNoise<N>(v, domainTRS, settings, config.C2.Seed) * config.C2.Displacement;
                noise2.Derivatives = derivativeMatrix.TransformVectors(noise2.Derivatives);
                
                Sample4 noise3 = Noise.GetFractalNoise<N>(v, domainTRS, settings, config.C3.Seed) * config.C3.Displacement;
                noise3.Derivatives = derivativeMatrix.TransformVectors(noise3.Derivatives);
                
                vertices[i] = SurfaceUtils.SetVertices(v, noise1 + noise2 + noise3, isPlane);
            }
        
            public static JobHandle ScheduleParallel (Mesh.MeshData meshData, int resolution, 
                Noise.Settings settings, SpaceTRS domain, Config config, bool isPlane,
                JobHandle dependency
            )
            {
                return new Job<N>
                {
                    vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
                    settings = settings,
                    domainTRS = domain.Matrix,
                    derivativeMatrix = domain.DerivativeMatrix,
                    config = config,
                    isPlane = isPlane
                }.ScheduleParallel(meshData.vertexCount / 4, resolution, dependency);
            }
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
        
        private float _localTime;
        
        protected override void Update()
        {
            _localTime += Time.deltaTime * _velocity;
            base.Update();
        }

        private const float ONE_THIRD = 1f / 3f;
        private const float TWO_THIRD = 2f / 3f;
        private const float FOUR_THIRD = 4f / 3f;

        private Config Config1(int seed, float displacement)
        {
            // note : expects ping pong animation curve

            float time = _localTime;
            int flooredTime = Mathf.FloorToInt(time);
            seed += flooredTime * 3;
            float fade = (time - flooredTime) * 2;
            
            float displacement1 = _lerpCurve.Evaluate(fade) * displacement;
            float displacement2 = _lerpCurve.Evaluate(fade + ONE_THIRD) * displacement;
            float displacement3 = _lerpCurve.Evaluate(fade + TWO_THIRD) * displacement;
            
            return new Config(
                new NoiseComponent(seed, displacement1), 
                new NoiseComponent(seed + 1, displacement2), 
                new NoiseComponent(seed + 2, displacement3)
            );
        }
        
        private Config Config2(int seed, float displacement)
        {
            // note : expects ping pong animation curve

            NoiseComponent GetNoiseComponent(int index)
            {
                float time = _localTime + index * ONE_THIRD;
                int flooredTime = Mathf.FloorToInt(time);
                return new NoiseComponent(seed + flooredTime + index, _lerpCurve.Evaluate((time - flooredTime) * 2) * displacement);
            }

            return new Config(GetNoiseComponent(0), GetNoiseComponent(1), GetNoiseComponent(2));
        }
        
        protected override JobHandle PerturbMesh(Mesh.MeshData meshData, int resolution, float displacement, NoiseType noiseType, int dimensions, Noise.Settings settings, int seed, SpaceTRS domain, JobHandle dependency)
        {
            Config config = Config2(seed, displacement);
            
            Debug.Log($"Scheduling {nameof(Config)} {config}");
            
            return _jobs[(int) noiseType, dimensions - 1](
                meshData,
                resolution,
                settings,
                domain,
                config,
                IsPlane,
                dependency);
        }
    }
}