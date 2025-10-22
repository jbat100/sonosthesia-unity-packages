using System;
using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sonosthesia.Mapping
{
    public enum FloatFaderType
    {
        Constant,
        Process
    }
    
    [Serializable]
    public class FloatFaderSettings 
    {
        [SerializeField] private FloatFaderType _faderType;

        [SerializeField] private float _constantValue;
        
        [SerializeField] private FloatProcessingType _processing;
        [SerializeField] private AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private RemapSettings _remap;
        [SerializeField] private FloatRange _clamp;
        
        public float Fade(float input) => _faderType switch
        {
            FloatFaderType.Constant => _constantValue,
            FloatFaderType.Process => _processing.ProcessFloat(input, _curve, _remap, _clamp),
            _ => 0
        };
    }
    
    public abstract class FaderConfiguration<T> : ScriptableObject where T : struct
    {
        public T Fade(float t) => PerformFade(t);

        protected abstract T PerformFade(float t);
    }
}