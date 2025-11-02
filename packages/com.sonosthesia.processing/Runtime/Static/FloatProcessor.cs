using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Processing
{
    [Flags]
    public enum FloatProcessingType
    {
        Curve = 1 << 0,
        Remap = 1 << 1,
        Clamp = 1 << 2,
        Abs = 1 << 3,
        Randomize = 1 << 4
    }

    public static class FloatProcessingExtensions
    {
        public static float ProcessFloat(this FloatProcessingType processingType, float value,
            AnimationCurve curve, RemapSettings remap, FloatRange clamp, float randomization)
        {
            if (processingType.HasFlag(FloatProcessingType.Curve))
            {
                value = curve.Evaluate(value);
            }

            if (processingType.HasFlag(FloatProcessingType.Remap))
            {
                value = remap.Remap(value);
            }

            if (processingType.HasFlag(FloatProcessingType.Clamp))
            {
                value = clamp.Clamp(value);
            }

            if (processingType.HasFlag(FloatProcessingType.Abs))
            {
                value = Mathf.Abs(value);
            }
            
            if (processingType.HasFlag(FloatProcessingType.Randomize))
            {
                value = value.NormalizedRandomization(randomization);
            }
            
            return value;
        }
    }
}