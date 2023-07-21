using UnityEngine;
using static UnityEngine.Mathf;


namespace Sonosthesia.Builder
{
    public static class FunctionLibrary
    {
        public enum FunctionName
        {
            Wave, 
            MultiWave, 
            Ripple
        }

        public delegate float Function (float x, float z, float t);

        private static readonly Function[] functions =
        {
            Wave, 
            MultiWave, 
            Ripple
        };
        
        public static Function GetFunction (FunctionName name) {
            return functions[(int)name];
        }
        
        private static float Wave (float x, float z, float t) 
        {
            return Sin(PI * (x + t));
        }
        
        private static float MultiWave (float x, float z, float t) 
        {
            return (Sin(PI * (x + 0.5f * t)) + 0.5f * Sin(2f * PI * (x + t))) * (2f / 3f);;
        }
        
        private static float Ripple (float x, float z, float t) 
        {
            float d = Abs(x);
            float y = Sin(PI * (4f * d - t));
            return y / (1f + 10f * d);
        }
    }
}