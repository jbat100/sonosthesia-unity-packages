using Unity.Mathematics;

namespace Sonosthesia.Utils
{
    public static class MathExtensions
    {
        public static float3 Horizontal(this float3 v)
        {
            return new float3(v.x, 0f, v.z);
        }
        
        public static float3 Vertical(this float3 v)
        {
            return new float3(0f, v.y, 0f);
        }
        
        // https://github.com/keijiro/ProceduralMotion/blob/master/Packages/jp.keijiro.klak.motion/Runtime/Internal/Utilities.cs
        public static Random Random(uint seed)
        {
            // Auto reseeding
            if (seed == 0) seed = (uint)UnityEngine.Random.Range(0, 0x7fffffff);

            var random = new Random(seed);

            // Abandon a few first numbers to warm up the PRNG.
            random.NextUInt();
            random.NextUInt();

            return random;
        }
        
        // https://github.com/keijiro/ProceduralMotion/blob/master/Packages/jp.keijiro.klak.motion/Runtime/BrownianMotion.cs
        public static float FractalBrownianMotion(float x, float y, int octave)
        {
            float2 p = math.float2(x, y);
            float f = 0.0f;
            float w = 0.5f;
            for (int i = 0; i < octave; i++)
            {
                f += w * noise.snoise(p);
                p *= 2.0f;
                w *= 0.5f;
            }
            return f;
        }

        public static void TestNoise()
        {
            float4 v4 = default;
            float3 v3 = default;
            noise.snoise(v3, out float3 gradient);
        }
    }
}