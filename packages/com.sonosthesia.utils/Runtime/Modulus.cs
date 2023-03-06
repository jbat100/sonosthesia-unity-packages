using UnityEngine;

namespace Sonosthesia.Utils
{
    public static class MathUtils
    {
        // https://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
        public static float Modulus(float a,float b)
        {
            return a - b * Mathf.FloorToInt(a / b);
        }
    }
}