using System.Runtime.CompilerServices;
using Sonosthesia.Noise;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Sonosthesia.Deform
{
    public static class SplineUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Sample4 GetFractalNoise<N>(SplineVertexData4 v, float3x4 domainTRS, FractalNoiseSettings settings, int seed) 
            where N : struct, INoise
        {
            return FractalNoise.GetFractalNoise<N>(
                domainTRS.TransformVectors(transpose(float3x4(
                    v.v0.position, v.v1.position, v.v2.position, v.v3.position
                ))),
                settings, seed
            );
        }
        
        public static SplineVertexData4 DeformVerticesAlongNormals (SplineVertexData4 v, Sample4 noise) 
        {
            // record original positions
            
            float4x3 p = transpose(float3x4(
                v.v0.position, v.v1.position, v.v2.position, v.v3.position
            ));
            
            // deform positions along normals
            
            v.v0.position += v.v0.normal * noise.v.x;
            v.v1.position += v.v1.normal * noise.v.y;
            v.v2.position += v.v2.normal * noise.v.z;
            v.v3.position += v.v3.normal * noise.v.w;

            // we apply the same trick as for the sphere but it probably doesn't work in the generic case
            
            //noise.dx /= noise.v;
            //noise.dy /= noise.v;
            //noise.dz /= noise.v;
            
            // this calculates the normal from the origin not the spline point, we need knowledge of this 
            // to be able to update the normals properly

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

            return v;
        }
    }
}