using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Plane = Unity.Mathematics.Geometry.Plane;

namespace Sonosthesia.Utils
{
    public static class MathExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float UnsafeSignedDistanceToPoint(this Plane plane, float3 point)
        {
            // same as SignedDistanceToPoint but avoids the Normalization check when not needed
            return math.dot(plane.NormalAndDistance, new float4(point, 1.0f));
        }
        
        // https://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
        public static float Modulus(float a,float b)
        {
            return a - b * Mathf.FloorToInt(a / b);
        }
        
        public static Vector3 Abs(this Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }
        
        public static float Sum(this Vector3 v) => v.x + v.y + v.z;

        public static float Sum(this Vector2 v) => v.x + v.y;

        public static float Average(this Vector3 v) => v.Sum() * 0.33333333333333f;

        public static float Average(this Vector2 v) => v.Sum() * 0.5f;

        public static float Max(this Vector3 v) => Mathf.Max(v.x, v.y, v.z);
        
        public static float Max(this Vector2 v) => Mathf.Max(v.x, v.y);
        
        public static float Min(this Vector3 v) => Mathf.Min(v.x, v.y, v.z);
        
        public static float Min(this Vector2 v) => Mathf.Min(v.x, v.y);

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            float inverseLerped = Mathf.InverseLerp(from1, to1, value);
            float remapped = Mathf.Lerp(from2, to2, inverseLerped);
            return remapped;
        }
        
        public static float3 Horizontal(this float3 v) => new (v.x, 0f, v.z);

        public static float3 Vertical(this float3 v) => new float3(0f, v.y, 0f);

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
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ChangeLength(this Vector3 vector, float factor)
        {
            if (vector == Vector3.zero)
            {
                return Vector3.zero;
            }
            float currentMagnitude = vector.magnitude;
            float newMagnitude = currentMagnitude * factor;
            return vector * (newMagnitude / currentMagnitude);
        }
        
        public static float DecibelToLinear(this float decibels) => math.pow(10f, decibels / 20f);
    }
}