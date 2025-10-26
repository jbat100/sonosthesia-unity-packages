using System;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [Serializable]
    public class MIDIPitchExtractorSettings<TEvent> : MIDIExtractorSettings, IMIDIPitchExtractor<TEvent>
        where TEvent : IInteractionEvent
    {
        [SerializeField][Range(0, 127)] private int _constant;

        [SerializeField] private InterfaceReference<IMIDIPitchExtractor<TEvent>> _extractor;
        
        [SerializeField] private InterfaceReference<IMIDIPitchProvider> _provider;
        
        public bool TryExtractMIDIPitch(TEvent e, out int val)
        {
            val = 0;
            switch (ExtractorType)
            {
                case MIDIExtractorType.Constant:
                    val = _constant;
                    return true;
                case MIDIExtractorType.Provider:
                    IMIDIPitchProvider provider = e.GetComponent(Origin, _provider.Value);
                    val = provider?.MIDIPitch ?? 0;
                    return provider != null;
                case MIDIExtractorType.Interactive:
                    IMIDIPitchExtractor<TEvent> extractor = e.GetComponent(Origin, _extractor.Value);
                    return extractor?.TryExtractMIDIPitch(e, out val) ?? false;
                default:
                    return false;
            }
        }
    }
}