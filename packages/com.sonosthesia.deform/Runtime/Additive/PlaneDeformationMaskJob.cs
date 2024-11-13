using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Sonosthesia.Ease;
using Sonosthesia.Mesh;

namespace Sonosthesia.Deform
{
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast)]
    public struct PlaneDeformationMaskJob : IJobFor
    {
        [ReadOnly] private NativeArray<Vertex4> vertices;
        [WriteOnly] private NativeArray<float4> mask;
        private float fade; // [0, 0.5]
        private EaseType easeType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Fade(float3 position)
        {
            float3 abs = math.abs(position) - 0.5f;
            float distance = math.min(abs.x, abs.y) / fade;
            if (distance > 1)
            {
                return 1;
            }
            return easeType.Evaluate(distance);
        }
        
        public void Execute(int index)
        {
            Vertex4 v = vertices[index];
            mask[index] = new float4(
                Fade(v.v0.position), 
                Fade(v.v1.position), 
                Fade(v.v2.position), 
                Fade(v.v3.position));
        }

        public static PlaneDeformationMaskJob Make(NativeArray<Vertex4> vertices, NativeArray<float4> mask,
            float fade, EaseType easeType)
        {
            return new PlaneDeformationMaskJob
            {
                vertices = vertices,
                mask = mask,
                fade = fade,
                easeType = easeType
            };
        }
        
    }
}