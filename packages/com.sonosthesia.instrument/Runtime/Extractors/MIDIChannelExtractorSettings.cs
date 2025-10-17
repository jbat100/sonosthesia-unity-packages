using System;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [Serializable]
    public abstract class MIDIChannelExtractorSettings<TEvent> : MIDIExtractorSettings, IMIDIChannelExtractor<TEvent>
    {
        [SerializeField] [Range(0, 15)] private int _constant;

        [SerializeField] private InterfaceReference<IMIDIChannelExtractor<TEvent>> _extractor;
        
        [SerializeField] private InterfaceReference<IMIDIChannelProvider> _provider;
        
        protected abstract GameObject GetSource(TEvent e);
        
        protected abstract GameObject GetActor(TEvent e);

        private IMIDIChannelExtractor<TEvent> GetExtractor(TEvent e) => origin switch
        {
            Origin.Self => _extractor.Value,
            Origin.Source => GetSource(e).GetComponent<IMIDIChannelExtractor<TEvent>>(),
            Origin.Actor => GetActor(e).GetComponent<IMIDIChannelExtractor<TEvent>>(),
            _ => null
        };
        
        private IMIDIChannelProvider GetProvider(TEvent e) => origin switch
        {
            Origin.Self => _provider.Value,
            Origin.Source => GetSource(e).GetComponent<IMIDIChannelProvider>(),
            Origin.Actor => GetActor(e).GetComponent<IMIDIChannelProvider>(),
            _ => null
        };
        
        public bool TryExtractMIDIChannel(TEvent e, out int val)
        {
            val = 0;
            switch (extractorType)
            {
                case ExtractorType.Constant:
                    val = _constant;
                    return true;
                case ExtractorType.Provider:
                    IMIDIChannelProvider provider = GetProvider(e);
                    val = provider?.MIDIChannel ?? 0;
                    return provider != null;
                case ExtractorType.Interactive:
                    IMIDIChannelExtractor<TEvent> extractor = GetExtractor(e);
                    return extractor?.TryExtractMIDIChannel(e, out val) ?? false;
                default:
                    return false;
            }
        }
    }
}