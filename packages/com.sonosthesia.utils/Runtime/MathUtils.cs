using Unity.Mathematics;

namespace Sonosthesia.Utils
{
    public static class MathUtils
    {
        // protesting against the absurd implementation of Mathf.InverseLerp
        public static float InverseLerp(float a, float b, float v)
        {
            return math.abs(b - a) < math.EPSILON ? 0f : math.clamp((v - a) / (b - a), 0f, 1f);
        }

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
    }
}