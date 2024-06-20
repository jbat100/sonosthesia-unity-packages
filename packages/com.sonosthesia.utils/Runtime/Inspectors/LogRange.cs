using System;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Sonosthesia.Utils
{
    // source : https://gist.github.com/bartofzo/6ad28a05ba9fc82e10a64f0c121c5c24
    
    /// <summary>
    /// Add this attribute to a float property to make it a logarithmic range slider
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class LogRangeAttribute : PropertyAttribute
    {
        public float min;
        public float center;
        public float max;

        /// <summary>
        /// Creates a float property slider with a logarithmic 
        /// </summary>
        /// <param name="min">Minimum range value</param>
        /// <param name="center">Value at the center of the range slider</param>
        /// <param name="max">Maximum range value</param>
        public LogRangeAttribute(float min, float center, float max)
        {
            this.min = min;
            this.center = center;
            this.max = max;
        }
    }
    
    /// <summary>
    /// Tool to convert a range from 0-1 into a logarithmic range with a user defined center
    /// </summary>
    public struct LogRangeConverter
    {
        public readonly float minValue;
        public readonly float maxValue;
        
        private readonly float a;
        private readonly float b;
        private readonly float c;

        /// <summary>
        /// Set up a scaler
        /// </summary>
        /// <param name="minValue">Value for t = 0</param>
        /// <param name="centerValue">Value for t = 0.5</param>
        /// <param name="maxValue">Value for t = 1.0</param>
        public LogRangeConverter(float minValue, float centerValue, float maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            
            a = (minValue * maxValue - (centerValue * centerValue)) / (minValue - 2 * centerValue + maxValue);
            b = ((centerValue - minValue) * (centerValue - minValue)) / (minValue - 2 * centerValue + maxValue);
            c = 2 * log((maxValue - centerValue) / (centerValue - minValue));
        }

        /// <summary>
        /// Convers the value in range 0 - 1 to the value in range of minValue - maxValue
        /// </summary>
        public float ToRange(float value01)
        {
            return a + b * exp(c * value01);
        }
        
        /// <summary>
        /// Converts the value in range min-max to a value between 0 and 1 that can be used for a slider
        /// </summary>
        public float ToNormalized(float rangeValue)
        {
            return log((rangeValue - a) / b) / c;
        }
    }
}