using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    // used for extracting a value based on a single TEvent, usually the first in the event stream 
    // for situations where after touch is not possible (e.g. MIDI note, velocity or channel selection) 
    
    [Serializable]
    public abstract class FloatStaticExtractorSettings<TEvent> : IStaticExtractor<TEvent, float> where TEvent : IInteractionEvent
    {
        [Flags]
        public enum PostProcessingType
        {
            Curve = 1 << 0,
            Remap = 1 << 1,
            Clamp = 1 << 2
        }
        
        // ----------- custom -------------
        
        [SerializeField] private StaticExtractor<TEvent, float> _extractor;
        
        // ----------- constant -------------
        
        [SerializeField] private float _constantValue;
        protected float ConstantValue => _constantValue;
        
        // ----------- postprocessing -------------
        
        [SerializeField] private PostProcessingType _postProcessing;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private RemapSettings _remap;
        [SerializeField] private FloatRange _clamp;
        
        protected bool Custom(TEvent e, out float value) => _extractor.Extract(e, out value);

        public bool Extract(TEvent e, out float value)
        {
            if (!ExtractRaw(e, out value))
            {
                return false;
            }

            value = PostProcess(value);
            return true;
        }

        protected abstract bool ExtractRaw(TEvent e, out float value);
        
        private float PostProcess(float value)
        {
            if (_postProcessing.HasFlag(PostProcessingType.Curve))
            {
                value = _curve.Evaluate(value);
            }

            if (_postProcessing.HasFlag(PostProcessingType.Remap))
            {
                value = _remap.Remap(value);
            }

            if (_postProcessing.HasFlag(PostProcessingType.Clamp))
            {
                value = _clamp.Clamp(value);
            }

            return value;
        }
    }
}