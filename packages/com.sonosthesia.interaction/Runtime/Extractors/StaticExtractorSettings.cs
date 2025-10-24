using UnityEngine;

namespace Sonosthesia.Interaction
{
    // used for extracting a value based on a single TEvent, usually the first in the event stream 
    // for situations where after touch is not possible (e.g. MIDI note, velocity or channel selection) 
    
    public abstract class StaticExtractorSettings<TEvent, TValue, TProcessing> : IStaticExtractor<TEvent, TValue>
        where TValue : struct
        where TProcessing : IPostProcessing<TValue>
    {
        [SerializeField] private TProcessing _postProcessing;
        
        [SerializeField] private InterfaceReference<IStaticExtractor<TEvent, TValue>> _extractor;
        
        [SerializeField] private TValue _constantValue;
        
        protected TValue ConstantValue => _constantValue;
        
        protected bool Custom(TEvent e, out TValue value) => _extractor.Value.Extract(e, out value);

        public bool Extract(TEvent e, out TValue value)
        {
            if (!ExtractRaw(e, out value))
            {
                return false;
            }

            value = _postProcessing.PostProcess(value);
            return true;
        }

        protected abstract bool ExtractRaw(TEvent e, out TValue value);
    }
}