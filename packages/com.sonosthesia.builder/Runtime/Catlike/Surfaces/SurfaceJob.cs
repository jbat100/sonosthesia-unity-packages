using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace Sonosthesia.Builder
{
    public delegate JobHandle SurfaceJobScheduleDelegate (
        Mesh.MeshData meshData, int resolution, Noise.Settings settings, SpaceTRS domain,
        float displacement, JobHandle dependency
    );
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct SurfaceJob<N> : IJobFor where N : struct, Noise.INoise
    {
        struct Vertex4 
        {
            public SingleStreams.Stream0 v0, v1, v2, v3;
        }

        private Noise.Settings settings;
        private float3x4 domainTRS;
        private float3x3 derivativeMatrix;
        private float displacement;

        private NativeArray<Vertex4> vertices;

        public void Execute (int i) 
        {
            Vertex4 v = vertices[i];
            Noise.Sample4 noise = Noise.GetFractalNoise<N>(
                domainTRS.TransformVectors(transpose(float3x4(
                    v.v0.position, v.v1.position, v.v2.position, v.v3.position
                ))),
                settings
            ) * displacement;
            
            v.v0.position.y = noise.v.x;
            v.v1.position.y = noise.v.y;
            v.v2.position.y = noise.v.z;
            v.v3.position.y = noise.v.w;
            
            float4x3 dNoise = derivativeMatrix.TransformVectors(noise.Derivatives);
            float4 normalizer = rsqrt(noise.dx * noise.dx + 1f);
            float4 tangentY = noise.dx * normalizer;
            v.v0.tangent = float4(normalizer.x, tangentY.x, 0f, -1f);
            v.v1.tangent = float4(normalizer.y, tangentY.y, 0f, -1f);
            v.v2.tangent = float4(normalizer.z, tangentY.z, 0f, -1f);
            v.v3.tangent = float4(normalizer.w, tangentY.w, 0f, -1f);
            
            normalizer = rsqrt(dNoise.c0 * dNoise.c0 + dNoise.c2 * dNoise.c2 + 1f);
            float4 normalX = -dNoise.c0 * normalizer;
            float4 normalZ = -dNoise.c2 * normalizer;
            v.v0.normal = float3(normalX.x, normalizer.x, normalZ.x);
            v.v1.normal = float3(normalX.y, normalizer.y, normalZ.y);
            v.v2.normal = float3(normalX.z, normalizer.z, normalZ.z);
            v.v3.normal = float3(normalX.w, normalizer.w, normalZ.w);

            vertices[i] = v;
        }
        
        public static JobHandle ScheduleParallel (Mesh.MeshData meshData, int resolution, 
            Noise.Settings settings, SpaceTRS domain, float displacement,
            JobHandle dependency
            ) => 
            new SurfaceJob<N>
            {
                vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
                settings = settings,
                domainTRS = domain.Matrix,
                derivativeMatrix = domain.DerivativeMatrix,
                displacement = displacement
            }.ScheduleParallel(meshData.vertexCount / 4, resolution, dependency);
    }    
}
