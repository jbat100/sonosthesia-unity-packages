using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Mapping
{
    public enum FloatFaderType
    {
        Remap,
        Curve
    }
    
    [Serializable]
    public class FloatFaderSettings
    {
        [SerializeField] private FloatFaderType _faderType;

        [SerializeField] private AnimationCurve _curve;
        
        [SerializeField] private FloatRange _remapInput = new FloatRange(0, 1);
        [SerializeField] private FloatRange _remapOutput = new FloatRange(0, 1);

        [SerializeField] private bool _clamp;
        [SerializeField] private FloatRange _clampRange = new FloatRange(0, 1);
        
        
        public float Fade(float input)
        {
            float result = _faderType switch
            {
                FloatFaderType.Remap => input.Remap(_remapInput, _remapOutput),
                FloatFaderType.Curve => _curve.Evaluate(input),
                _ => 0
            };

            if (_clamp)
            {
                result = result.Clamp(_clampRange);
            }

            return result;
        }
    }
    
    public abstract class FaderConfiguration<T> : ScriptableObject where T : struct
    {
        public T Fade(float t) => PerformFade(t);

        protected abstract T PerformFade(float t);
    }
}