using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace Sonosthesia.Builder
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct DeformMeshJob : IJobFor
    {
        struct Vertex4 
        {
            public SingleStreams.Stream0 v0, v1, v2, v3;
        }

        private float3x3 derivativeMatrix;
        private float displacement;
        private bool isPlane;
        private NativeArray<Vertex4> vertices;
        private NativeArray<Sample4> defomations;

        public void Execute(int i)
        {
            Vertex4 v = vertices[i];
            Sample4 noise = defomations[i]  * displacement;
            noise.Derivatives = derivativeMatrix.TransformVectors(noise.Derivatives);
            if (isPlane) 
            {
                vertices[i] = SetPlaneVertices(v, noise);
            }
            else 
            {
                vertices[i] = SetSphereVertices(v, noise);
            }
        }
        
        private Vertex4 SetPlaneVertices (Vertex4 v, Sample4 noise) 
        {
            v.v0.position.y = noise.v.x;
            v.v1.position.y = noise.v.y;
            v.v2.position.y = noise.v.z;
            v.v3.position.y = noise.v.w;
            
            float4 normalizer = rsqrt(noise.dx * noise.dx + 1f);
            float4 tangentY = noise.dx * normalizer;
            v.v0.tangent = float4(normalizer.x, tangentY.x, 0f, -1f);
            v.v1.tangent = float4(normalizer.y, tangentY.y, 0f, -1f);
            v.v2.tangent = float4(normalizer.z, tangentY.z, 0f, -1f);
            v.v3.tangent = float4(normalizer.w, tangentY.w, 0f, -1f);
            
            normalizer = rsqrt(noise.dx * noise.dx + noise.dz * noise.dz + 1f);
            float4 normalX = -noise.dx * normalizer;
            float4 normalZ = -noise.dz * normalizer;
            v.v0.normal = float3(normalX.x, normalizer.x, normalZ.x);
            v.v1.normal = float3(normalX.y, normalizer.y, normalZ.y);
            v.v2.normal = float3(normalX.z, normalizer.z, normalZ.z);
            v.v3.normal = float3(normalX.w, normalizer.w, normalZ.w);

            return v;
        }
        
        private Vertex4 SetSphereVertices (Vertex4 v, Sample4 noise) 
        {
            noise.v += 1f;
            noise.dx /= noise.v;
            noise.dy /= noise.v;
            noise.dz /= noise.v;
            
            float4x3 p = transpose(float3x4(
                v.v0.position, v.v1.position, v.v2.position, v.v3.position
            ));
            
            float3 tangentCheck = abs(v.v0.tangent.xyz);
            if (tangentCheck.x + tangentCheck.y + tangentCheck.z > 0f)
            {
                float4x3 t = transpose(float3x4(
                    v.v0.tangent.xyz, v.v1.tangent.xyz, v.v2.tangent.xyz, v.v3.tangent.xyz
                ));

                float4 td = t.c0 * noise.dx + t.c1 * noise.dy + t.c2 * noise.dz;
                t.c0 += td * p.c0;
                t.c1 += td * p.c1;
                t.c2 += td * p.c2;
            
                float3x4 tt = transpose(t.NormalizeRows());
                v.v0.tangent = float4(tt.c0, -1f);
                v.v1.tangent = float4(tt.c1, -1f);
                v.v2.tangent = float4(tt.c2, -1f);
                v.v3.tangent = float4(tt.c3, -1f);                
            }

            float4 pd = p.c0 * noise.dx + p.c1 * noise.dy + p.c2 * noise.dz;
            float3x4 nt = transpose(float4x3(
                p.c0 - noise.dx + pd * p.c0,
                p.c1 - noise.dy + pd * p.c1,
                p.c2 - noise.dz + pd * p.c2
            ).NormalizeRows());
            
            v.v0.normal = nt.c0;
            v.v1.normal = nt.c1;
            v.v2.normal = nt.c2;
            v.v3.normal = nt.c3;
            
            v.v0.position *= noise.v.x;
            v.v1.position *= noise.v.y;
            v.v2.position *= noise.v.z;
            v.v3.position *= noise.v.w;

            return v;
        }
        
        public static JobHandle ScheduleParallel (Mesh.MeshData meshData, NativeArray<Sample4> defomations, 
            SpaceTRS domain, float displacement, bool isPlane, int innerloopBatchCount, JobHandle dependency
            ) => new DeformMeshJob()
            {
                vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
                defomations = defomations,
                derivativeMatrix = domain.DerivativeMatrix,
                displacement = displacement,
                isPlane = isPlane
            }.ScheduleParallel(meshData.vertexCount / 4, innerloopBatchCount, dependency);
    }    
}