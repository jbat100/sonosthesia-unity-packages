using System;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Utils
{
    [Serializable]
    public struct FloatRange
    {
        [SerializeField] private float _min;
        public float Min => _min;
        
        [SerializeField] private float _max;
        public float Max => _max;

        public FloatRange(float min, float max)
        {
            _min = min;
            _max = max;
        }
    }

    public static class FloatRangeExtensions
    {
        public static float Remap(this float value, FloatRange inputRange, FloatRange outputRange)
        {
            float inputWidth = inputRange.Max - inputRange.Min;
            
            if (math.abs(inputWidth) < 1e-6)
            {
                return outputRange.Min;
            }
            
            // Calculate the normalized value (0 to 1) within the input range
            float t = (value - inputRange.Min) / (inputRange.Max - inputRange.Min);
        
            // Map the normalized value to the output range
            return outputRange.Min + t * (outputRange.Max - outputRange.Min);
        }

        public static float Clamp(this float value, FloatRange range)
        {
            return math.clamp(value, range.Min, range.Max);
        }
    }
}