using System;
using Sonosthesia.Processing;
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
        
        [SerializeField] private FloatProcessorSettings _processorSettings;
        
        public float Fade(float input) => _faderType switch
        {
            FloatFaderType.Constant => _constantValue,
            FloatFaderType.Process => _processorSettings.Process(input),
            _ => 0
        };
    }
    
    public abstract class FaderConfiguration<T> : ScriptableObject where T : struct
    {
        public T Fade(float t) => PerformFade(t);

        protected abstract T PerformFade(float t);
    }
}