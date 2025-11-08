using System;
using Sonosthesia.Processing;
using UnityEngine;

namespace Sonosthesia.Extractor
{
    // used for extracting a value based on a single TEvent, usually the first in the event stream 
    // for situations where after touch is not possible (e.g. MIDI note, velocity or channel selection) 
    
    [Serializable]
    public abstract class StaticExtractorSettings<TEvent, TValue, TProcessor> : IStaticExtractor<TEvent, TValue>
        where TValue : struct
        where TProcessor : IProcessor<TValue>
    {
        [SerializeField] private TProcessor _processor;
        
        [SerializeField] private InterfaceReference<IStaticExtractor<TEvent, TValue>> _extractor;
        
        [SerializeField] private TValue _constantValue;
        
        protected bool ExtractCustom(TEvent e, out TValue value) => _extractor.Value.Extract(e, out value);

        protected bool ExtractConstant(TEvent e, out TValue value)
        {
            value = _constantValue;
            return true; 
        }

        public bool Extract(TEvent e, out TValue value)
        {
            if (!ExtractRaw(e, out value))
            {
                return false;
            }

            value = _processor.Process(value);
            return true;
        }

        protected abstract bool ExtractRaw(TEvent e, out TValue value);
    }

}