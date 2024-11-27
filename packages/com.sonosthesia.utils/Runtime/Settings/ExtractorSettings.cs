using System;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public interface IExtractor<in T> where T : struct
    {
        float Extract(T input);
    }
    
    [Flags]
    public enum ExtractorPostProcessingType
    {
        Remap = 1 << 0,
        Clamp = 1 << 1,
        Randomize = 1 << 2
    }
    
    [Serializable]
    public abstract class ExtractorSettings<T, TSelector> : IExtractor<T> where T : struct
    {
        [SerializeField] private TSelector _selector;
        [SerializeField] private ExtractorPostProcessingType _postProcessing;
        [SerializeField] private RemapSettings _remap;
        [SerializeField] private FloatRange _clamp;
        [SerializeField][Range(0, 1)] private float _randomize;

        public float Extract(T input)
        {
            float result = Select(input, _selector);
            
            if (_postProcessing.HasFlag(ExtractorPostProcessingType.Remap))
            {
                result = _remap.Remap(result);
            }

            if (_postProcessing.HasFlag(ExtractorPostProcessingType.Clamp))
            {
                result = _clamp.Clamp(result);
            }

            if (_postProcessing.HasFlag(ExtractorPostProcessingType.Randomize))
            {
                result = result.NormalizedRandomization(_randomize);
            }

            return result;
        }

        protected abstract float Select(T input, TSelector selector);
    }
}