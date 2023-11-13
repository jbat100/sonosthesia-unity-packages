using System.Runtime.CompilerServices;
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
    public static class MultiNoise
    {
        [SerializeField]
        public struct HarmonicSettings
        {
            public bool active;
            
            [Min(1)] public int frequency;
            [Range(0f, 1f)] public float displacement;
            
            public static HarmonicSettings Default => new HarmonicSettings
            {
                frequency = 4,
                displacement = 1
            };
        }

        [SerializeField]
        public struct DynamicSettings
        {
            public HarmonicSettings settings;
            public SmallXXHash4 hash1;
            public SmallXXHash4 hash2;
            public float lerp;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Sample4 GetNoise<N>(float4x3 position, HarmonicSettings settings, SmallXXHash4 hash) where N : struct, Noise.INoise
        {
            return settings.displacement * default(N).GetNoise4(position, hash, settings.frequency);;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Sample4 GetNoise<N>(float4x3 position, DynamicSettings settings) where N : struct, Noise.INoise
        {
            Sample4 noise1 = GetNoise<N>(position, settings.settings, settings.hash1);
            Sample4 noise2 = GetNoise<N>(position, settings.settings, settings.hash2);
            return noise1 * (1 - settings.lerp) + noise2 * settings.lerp;
        }

        [SerializeField]
        public struct DynamicFractalSettings
        {
            public FractalSettings settings;
            public int seed;
            public float lerp;
            public float displacement;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Sample4 GetFractalNoise<N>(float4x3 position, DynamicFractalSettings settings) where N : struct, Noise.INoise
        {
            Sample4 noise1 = Noise.Noise.GetFractalNoise<N>(position, settings.settings, settings.seed);
            Sample4 noise2 = Noise.Noise.GetFractalNoise<N>(position, settings.settings, settings.seed + 1);
            return (noise1 * (1 - settings.lerp) + noise2 * settings.lerp) * settings.displacement;
        }

        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        public struct FractalJob4<N1, N2, N3, N4> : IJobFor
            where N1 : struct, Noise.INoise
            where N2 : struct, Noise.INoise
            where N3 : struct, Noise.INoise
            where N4 : struct, Noise.INoise
        {
            private DynamicFractalSettings settings1;
            private DynamicFractalSettings settings2;
            private DynamicFractalSettings settings3;
            private DynamicFractalSettings settings4;
            
            private float3x4 domainTRS;
            private float3x3 derivativeMatrix;
            private bool isPlane;
            private NativeArray<Vertex4> vertices;

            public void Execute(int i)
            {
                Vertex4 v = vertices[i];

                float4x3 position = domainTRS.TransformVectors(transpose(float3x4(
                    v.v0.position, v.v1.position, v.v2.position, v.v3.position
                )));
                
                Sample4 noise1 = GetFractalNoise<N1>(position, settings1);
                noise1.Derivatives = derivativeMatrix.TransformVectors(noise1.Derivatives);
                
                Sample4 noise2 = GetFractalNoise<N1>(position, settings2);
                noise2.Derivatives = derivativeMatrix.TransformVectors(noise2.Derivatives);
                
                Sample4 noise3 = GetFractalNoise<N1>(position, settings3);
                noise3.Derivatives = derivativeMatrix.TransformVectors(noise3.Derivatives);
                
                Sample4 noise4 = GetFractalNoise<N1>(position, settings4);
                noise4.Derivatives = derivativeMatrix.TransformVectors(noise4.Derivatives);
                
                vertices[i] = SurfaceUtils.SetVertices(v, noise1 + noise2 + noise3 + noise4, isPlane);
            }
        
            public static JobHandle ScheduleParallel (UnityEngine.Mesh.MeshData meshData, int resolution, 
                DynamicFractalSettings settings1,
                DynamicFractalSettings settings2,
                DynamicFractalSettings settings3,
                DynamicFractalSettings settings4,
                SpaceTRS domain, bool isPlane, 
                JobHandle dependency
            ) => 
                new FractalJob4<N1, N2, N3, N4>
                {
                    vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
                    settings1 = settings1,
                    settings2 = settings2,
                    settings3 = settings3,
                    settings4 = settings4,
                    domainTRS = domain.Matrix,
                    derivativeMatrix = domain.DerivativeMatrix,
                    isPlane = isPlane
                }.ScheduleParallel(meshData.vertexCount / 4, resolution, dependency);
        }    
    }
}