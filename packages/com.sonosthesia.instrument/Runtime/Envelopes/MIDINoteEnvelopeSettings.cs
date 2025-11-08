using System;
using Sonosthesia.Interaction;
using Sonosthesia.MIDI;

namespace Sonosthesia.Instrument
{
    [Serializable]
    public class MIDINoteEnvelopeSettings : InteractiveEnvelopeSettings<MIDINote, 
        FloatMIDINoteDynamicExtractorSettings, 
        FloatMIDINoteStaticExtractorSettings>
    {
        
    }
}