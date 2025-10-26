using System;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [Serializable]
    public abstract class MIDIChannelExtractorSettings<TEvent> : MIDIExtractorSettings, IMIDIChannelExtractor<TEvent>
        where TEvent : IInteractionEvent
    {
        [SerializeField] [Range(0, 15)] private int _constant;

        [SerializeField] private InterfaceReference<IMIDIChannelExtractor<TEvent>> _extractor;
        
        [SerializeField] private InterfaceReference<IMIDIChannelProvider> _provider;
        
        public bool TryExtractMIDIChannel(TEvent e, out int val)
        {
            val = 0;
            switch (ExtractorType)
            {
                case MIDIExtractorType.Constant:
                    val = _constant;
                    return true;
                case MIDIExtractorType.Provider:
                    IMIDIChannelProvider provider = e.GetComponent(Origin, _provider.Value);;
                    val = provider?.MIDIChannel ?? 0;
                    return provider != null;
                case MIDIExtractorType.Interactive:
                    IMIDIChannelExtractor<TEvent> extractor = e.GetComponent(Origin, _extractor.Value);;
                    return extractor?.TryExtractMIDIChannel(e, out val) ?? false;
                default:
                    return false;
            }
        }
    }
}