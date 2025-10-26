using System;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public enum MIDIExtractorType
    {
        Constant,
        Provider,
        Interactive
    }
    
    [Serializable]
    public class MIDIExtractorSettings
    {
        [SerializeField] private MIDIExtractorType _extractorType;
        protected MIDIExtractorType ExtractorType => _extractorType;
        
        [SerializeField] private InteractionExtractorOrigin _origin = InteractionExtractorOrigin.Self;
        protected InteractionExtractorOrigin Origin => _origin;
    }
}