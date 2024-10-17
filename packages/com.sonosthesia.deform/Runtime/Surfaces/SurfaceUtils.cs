using Sonosthesia.Noise;
using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace Sonosthesia.Deform
{
    public static class SurfaceUtils
    {
        public static Vertex4 SetDeformation(Vertex4 v, float4 noise, bool isPlane)
        {
            return isPlane ? SetPlaneDeformation(v, noise) : SetSphereDeformations(v, noise);
        }
        
        public static Vertex4 SetPlaneDeformation(Vertex4 v, float4 noise)
        {
            v.v0.position.y = noise.x;
            v.v1.position.y = noise.y;
            v.v2.position.y = noise.z;
            v.v3.position.y = noise.w;

            return v;
        }

        public static Vertex4 SetSphereDeformations(Vertex4 v, float4 noise)
        {
            noise += 1f;
            
            v.v0.position *= noise.x;
            v.v1.position *= noise.y;
            v.v2.position *= noise.z;
            v.v3.position *= noise.w;

            return v;
        }
        
        public static Vertex4 SetVertices(Vertex4 v, Sample4 noise, bool isPlane)
        {
            return isPlane ? SetPlaneVertices(v, noise) : SetSphereVertices(v, noise);
        }
        
        public static Vertex4 SetPlaneVertices (Vertex4 v, Sample4 noise) 
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
        
        public static Vertex4 SetSphereVertices (Vertex4 v, Sample4 noise) 
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
    }
}