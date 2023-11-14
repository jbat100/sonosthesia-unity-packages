using Sonosthesia.Mesh;
using Sonosthesia.Noise;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace Sonosthesia.Deform
{
    public class TriFractalMeshNoiseController : FractalMeshNoiseController
    {
        protected override bool IsDynamic => true;

        [SerializeField] private float _velocity = 1f;

        [SerializeField] private AnimationCurve _lerpCurve;

        private delegate JobHandle JobScheduleDelegate (
            UnityEngine.Mesh.MeshData meshData, int resolution, FractalNoiseSettings settings, SpaceTRS domain,
            TriNoise.TriNoisePhase triPhase, bool isPlane, JobHandle dependency
        );
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct Job<N> : IJobFor where N : struct, INoise
        {
            private FractalNoiseSettings settings;
            private float3x4 domainTRS;
            private float3x3 derivativeMatrix;
            private TriNoise.TriNoisePhase triPhase;
            private bool isPlane;
            private NativeArray<Vertex4> vertices;

            public void Execute(int i)
            {
                Vertex4 v = vertices[i];
                
                float4x3 position = domainTRS.TransformVectors(transpose(float3x4(
                    v.v0.position, v.v1.position, v.v2.position, v.v3.position
                )));

                Sample4 noise = position.GetFractalNoise<N>(settings, triPhase, derivativeMatrix);
                
                vertices[i] = SurfaceUtils.SetVertices(v, noise, isPlane);
            }
        
            public static JobHandle ScheduleParallel (UnityEngine.Mesh.MeshData meshData, int resolution, 
                FractalNoiseSettings settings, SpaceTRS domain, TriNoise.TriNoisePhase triPhase, bool isPlane,
                JobHandle dependency
            )
            {
                return new Job<N>
                {
                    vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
                    settings = settings,
                    domainTRS = domain.Matrix,
                    derivativeMatrix = domain.DerivativeMatrix,
                    triPhase = triPhase,
                    isPlane = isPlane
                }.ScheduleParallel(meshData.vertexCount / 4, resolution, dependency);
            }
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
        
        private float _localTime;
        
        protected override void Update()
        {
            _localTime += Time.deltaTime * _velocity;
            base.Update();
        }

        private const float ONE_THIRD = 1f / 3f;

        protected override JobHandle PerturbMesh(UnityEngine.Mesh.MeshData meshData, int resolution, float displacement, NoiseType noiseType, int dimensions, FractalNoiseSettings settings, int seed, SpaceTRS domain, JobHandle dependency)
        {
            TriNoise.NoisePhase GetPhase(int index)
            {
                float time = _localTime + index * ONE_THIRD;
                int flooredTime = Mathf.FloorToInt(time);
                return new TriNoise.NoisePhase(seed + flooredTime + index, _lerpCurve.Evaluate((time - flooredTime) * 2) * displacement);
            }

            TriNoise.TriNoisePhase triPhase = new TriNoise.TriNoisePhase(GetPhase(0), GetPhase(1), GetPhase(2));

            Debug.Log($"Scheduling {nameof(TriNoise.TriNoisePhase)} {triPhase}");
            
            return _jobs[(int) noiseType, dimensions - 1](
                meshData,
                resolution,
                settings,
                domain,
                triPhase,
                IsPlane,
                dependency);
        }
    }
}