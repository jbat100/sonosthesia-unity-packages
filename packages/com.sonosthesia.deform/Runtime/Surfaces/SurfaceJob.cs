using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace Sonosthesia.Builder
{
    public delegate JobHandle SurfaceJobScheduleDelegate (
        Mesh.MeshData meshData, int resolution, Noise.Settings settings, int seed, SpaceTRS domain,
        float displacement, bool isPlane, JobHandle dependency
    );
    
    public struct Vertex4 
    {
        public SingleStreams.Stream0 v0, v1, v2, v3;
    }

    public static class Vertex4Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Sample4 GetFractalNoise<N>(this Vertex4 v, float3x4 domainTRS, Noise.Settings settings, int seed) 
            where N : struct, Noise.INoise
        {
            return Noise.GetFractalNoise<N>(
                domainTRS.TransformVectors(transpose(float3x4(
                    v.v0.position, v.v1.position, v.v2.position, v.v3.position
                ))),
                settings, seed
            );
        }   
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 GetSimpleFractalNoise<N>(this Vertex4 v, float3x4 domainTRS, Noise.Settings settings, int seed) 
            where N : struct, Noise.ISimpleNoise
        {
            return Noise.GetSimpleFractalNoise<N>(
                domainTRS.TransformVectors(transpose(float3x4(
                    v.v0.position, v.v1.position, v.v2.position, v.v3.position
                ))),
                settings, seed
            );
        }
    }

    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct SurfaceJob<N> : IJobFor where N : struct, Noise.INoise
    {
        private Noise.Settings settings;
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
        
        public static JobHandle ScheduleParallel (Mesh.MeshData meshData, int resolution, 
            Noise.Settings settings, int seed, SpaceTRS domain, float displacement, bool isPlane, 
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
    }
}
