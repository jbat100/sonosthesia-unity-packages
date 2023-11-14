using System.Runtime.CompilerServices;
using Sonosthesia.Mesh;
using Sonosthesia.Noise;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace Sonosthesia.Deform
{
    public delegate JobHandle SurfaceJobScheduleDelegate (
        UnityEngine.Mesh.MeshData meshData, int resolution, FractalNoiseSettings settings, int seed, SpaceTRS domain,
        float displacement, bool isPlane, JobHandle dependency
    );
    
    public struct Vertex4 
    {
        public SingleStreams.Stream0 v0, v1, v2, v3;
    }

    public static class Vertex4Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Sample4 GetFractalNoise<N>(this Vertex4 v, float3x4 domainTRS, FractalNoiseSettings settings, int seed) 
            where N : struct, INoise
        {
            return FractalNoise.GetFractalNoise<N>(
                domainTRS.TransformVectors(transpose(float3x4(
                    v.v0.position, v.v1.position, v.v2.position, v.v3.position
                ))),
                settings, seed
            );
        }   
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 GetSimpleFractalNoise<N>(this Vertex4 v, float3x4 domainTRS, FractalNoiseSettings settings, int seed) 
            where N : struct, ISimpleNoise
        {
            return FractalNoise.GetSimpleFractalNoise<N>(
                domainTRS.TransformVectors(transpose(float3x4(
                    v.v0.position, v.v1.position, v.v2.position, v.v3.position
                ))),
                settings, seed
            );
        }
    }

    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct SurfaceJob<N> : IJobFor where N : struct, INoise
    {
        private FractalNoiseSettings settings;
        private int seed;
        private float3x4 domainTRS;
        private float3x3 derivativeMatrix;
        private float displacement;
        private bool isPlane;
        private NativeArray<Vertex4> vertices;

        public void Execute(int i)
        {
            Vertex4 v = vertices[i];
            Sample4 noise = v.GetFractalNoise<N>(domainTRS, settings, seed) * displacement;
            noise.Derivatives = derivativeMatrix.TransformVectors(noise.Derivatives);
            vertices[i] = SurfaceUtils.SetVertices(v, noise, isPlane);
        }
        
        public static JobHandle ScheduleParallel (UnityEngine.Mesh.MeshData meshData, int resolution, 
            FractalNoiseSettings settings, int seed, SpaceTRS domain, float displacement, bool isPlane, 
            JobHandle dependency
            ) => 
            new SurfaceJob<N>
            {
                vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
                settings = settings,
                seed = seed,
                domainTRS = domain.Matrix,
                derivativeMatrix = domain.DerivativeMatrix,
                displacement = displacement,
                isPlane = isPlane
            }.ScheduleParallel(meshData.vertexCount / 4, resolution, dependency);
    }

    public static class SurfaceJob
    {
        public static readonly SurfaceJobScheduleDelegate[,] Jobs = {
            {
                SurfaceJob<Lattice1D<Perlin, LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Lattice2D<Perlin, LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Lattice3D<Perlin, LatticeNormal>>.ScheduleParallel
            },
            {
                SurfaceJob<Lattice1D<Smoothstep<Turbulence<Perlin>>, LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Lattice2D<Smoothstep<Turbulence<Perlin>>, LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Lattice3D<Smoothstep<Turbulence<Perlin>>, LatticeNormal>>.ScheduleParallel
            },
            {
                SurfaceJob<Lattice1D<Value, LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Lattice2D<Value, LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Lattice3D<Value, LatticeNormal>>.ScheduleParallel
            },
            {
                SurfaceJob<Simplex1D<Simplex>>.ScheduleParallel,
                SurfaceJob<Simplex2D<Simplex>>.ScheduleParallel,
                SurfaceJob<Simplex3D<Simplex>>.ScheduleParallel
            },
            {
                SurfaceJob<Simplex1D<Turbulence<Simplex>>>.ScheduleParallel,
                SurfaceJob<Simplex2D<Turbulence<Simplex>>>.ScheduleParallel,
                SurfaceJob<Simplex3D<Turbulence<Simplex>>>.ScheduleParallel
            },
            {
                SurfaceJob<Simplex1D<Smoothstep<Turbulence<Simplex>>>>.ScheduleParallel,
                SurfaceJob<Simplex2D<Smoothstep<Turbulence<Simplex>>>>.ScheduleParallel,
                SurfaceJob<Simplex3D<Smoothstep<Turbulence<Simplex>>>>.ScheduleParallel
            },
            {
                SurfaceJob<Simplex1D<Value>>.ScheduleParallel,
                SurfaceJob<Simplex2D<Value>>.ScheduleParallel,
                SurfaceJob<Simplex3D<Value>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, Worley, F1>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, Worley, F1>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, Worley, F1>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, Worley, F2>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, Worley, F2>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, Worley, F2>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, SmoothWorley, F1>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, SmoothWorley, F1>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, SmoothWorley, F1>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, SmoothWorley, F2>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, SmoothWorley, F2>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, SmoothWorley, F2>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel
            }
        };
    }
}
