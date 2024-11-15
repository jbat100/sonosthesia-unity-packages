using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Sonosthesia.Utils
{
    public static class MathUtils
    {
        private static Random _random = new Random(12345);

        public static float RandomFloat() => _random.NextFloat();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RandomFloat(float lower, float upper)
        {
            if (lower >= upper)
            {
                return lower;
            }

            return lower + RandomFloat() * (upper - lower);
        }

        public static float NormalizedRandomization(this float input, float normalizedRandomization)
        {
            return input + RandomFloat(-1, 1) * input * math.clamp(normalizedRandomization, 0, 1);
        }
        
        // protesting against the absurd implementation of Mathf.InverseLerp (and the wasteful nature of Mathf generally)
        
        public static float InverseLerp(float a, float b, float v)
        {
            return math.abs(b - a) < math.EPSILON ? 0f : math.clamp((v - a) / (b - a), 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Remap(float input, float fromMin, float fromMax, float toMin, float toMax, bool clamp)
        {
            if (math.abs(fromMax - fromMin) < 1e-6)
            {
                return toMin;
            }
            float t = math.unlerp(fromMin, fromMax, input);
            float result = math.lerp(toMin, toMax, t);
            if (clamp)
            {
                result = math.clamp(result, math.min(toMin, toMax), math.max(toMin, toMax));
            }

            return result;
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
    }
}