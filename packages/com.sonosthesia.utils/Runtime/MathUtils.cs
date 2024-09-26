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
    }
}