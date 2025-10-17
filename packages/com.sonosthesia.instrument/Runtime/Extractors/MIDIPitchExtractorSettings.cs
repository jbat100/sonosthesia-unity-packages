using System;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [Serializable]
    public abstract class MIDIPitchExtractorSettings<TEvent> : MIDIExtractorSettings, IMIDIPitchExtractor<TEvent>
    {
        [SerializeField][Range(0, 127)] private int _constant;

        [SerializeField] private InterfaceReference<IMIDIPitchExtractor<TEvent>> _extractor;
        
        [SerializeField] private InterfaceReference<IMIDIPitchProvider> _provider;
        
        protected abstract GameObject GetSource(TEvent e);
        
        protected abstract GameObject GetActor(TEvent e);

        private IMIDIPitchExtractor<TEvent> GetExtractor(TEvent e) => origin switch
        {
            Origin.Self => _extractor.Value,
            Origin.Source => GetSource(e).GetComponent<IMIDIPitchExtractor<TEvent>>(),
            Origin.Actor => GetActor(e).GetComponent<IMIDIPitchExtractor<TEvent>>(),
            _ => null
        };
        
        private IMIDIPitchProvider GetProvider(TEvent e) => origin switch
        {
            Origin.Self => _provider.Value,
            Origin.Source => GetSource(e).GetComponent<IMIDIPitchProvider>(),
            Origin.Actor => GetActor(e).GetComponent<IMIDIPitchProvider>(),
            _ => null
        };
        
        public bool TryExtractMIDIPitch(TEvent e, out int val)
        {
            val = 0;
            switch (extractorType)
            {
                case ExtractorType.Constant:
                    val = _constant;
                    return true;
                case ExtractorType.Provider:
                    IMIDIPitchProvider provider = GetProvider(e);
                    val = provider?.MIDIPitch ?? 0;
                    return provider != null;
                case ExtractorType.Interactive:
                    IMIDIPitchExtractor<TEvent> extractor = GetExtractor(e);
                    return extractor?.TryExtractMIDIPitch(e, out val) ?? false;
                default:
                    return false;
            }
        }
    }
}