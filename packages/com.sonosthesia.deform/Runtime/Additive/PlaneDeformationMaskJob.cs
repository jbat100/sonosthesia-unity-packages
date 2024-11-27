using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Sonosthesia.Ease;

namespace Sonosthesia.Deform
{
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast)]
    public struct PlaneDeformationMaskJob : IJobFor
    {
        [ReadOnly] public NativeArray<Vertex4> vertices;
        [WriteOnly] public NativeArray<float4> mask;
        public float fade; // [0, 0.5]
        public EaseType easeType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float2 Distance(float2 pos)
        {
            float2 pos01 = pos + 0.5f;
            return math.abs(pos01 - math.round(pos01));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Fade(float3 position)
        {
            float2 distanceXZ = Distance(new float2(position.x, position.z));
            float distance = math.min(distanceXZ.x, distanceXZ.y);
            float f = math.unlerp(0, fade, distance);
            if (f > 1)
            {
                return 1;
            }
            return easeType.Evaluate(f);
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