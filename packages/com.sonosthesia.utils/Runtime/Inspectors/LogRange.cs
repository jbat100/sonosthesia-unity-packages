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
}