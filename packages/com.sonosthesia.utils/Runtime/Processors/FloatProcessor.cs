using System;
using UnityEngine;

namespace Sonosthesia.Utils
{
    [Flags]
    public enum FloatProcessingType
    {
        Curve = 1 << 0,
        Remap = 1 << 1,
        Clamp = 1 << 2,
        Abs = 1 << 3,
    }

    public static class FloatProcessingExtensions
    {
        public static float ProcessFloat(this FloatProcessingType processingType, float value,
            AnimationCurve curve, RemapSettings remap, FloatRange clamp)
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
            
            return value;
        }
    }
    
    [Serializable]
    public class FloatProcessor : IProcessor<float>
    {
        [SerializeField] private bool _abs;
        
        [SerializeField] private float _scale = 1f;

        [SerializeField] private float _offset;

        [SerializeField] private bool _clamp;

        [SerializeField] private Vector2 _clampRange;
        
        public float Process(float value)
        {
            float result = value;

            if (_abs)
            {
                result = _abs ? Mathf.Abs(value) : value;
            }
            
            result = _offset + result * _scale;

            if (_clamp)
            {
                result = Mathf.Clamp(result, _clampRange.x, _clampRange.y);
            }

            return result;
        }
    }
}