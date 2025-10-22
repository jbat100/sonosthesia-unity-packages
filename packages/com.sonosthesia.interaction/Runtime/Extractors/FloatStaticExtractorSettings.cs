using System;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    // used for extracting a value based on a single TEvent, usually the first in the event stream 
    // for situations where after touch is not possible (e.g. MIDI note, velocity or channel selection) 
    
    [Serializable]
    public abstract class FloatStaticExtractorSettings<TEvent> : FloatPostProcessingSettings, IStaticExtractor<TEvent, float>
    {
        [SerializeField] private InterfaceReference<IStaticExtractor<TEvent, float>> _extractor;
        
        [SerializeField] private float _constantValue;
        protected float ConstantValue => _constantValue;
        
        protected bool Custom(TEvent e, out float value) => _extractor.Value.Extract(e, out value);

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
    }
}