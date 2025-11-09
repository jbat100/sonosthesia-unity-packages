using Sonosthesia.Interaction;
using Sonosthesia.MIDI;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [CreateAssetMenu(fileName = "MIDINoteTriggerConfiguration", 
        menuName = "Sonosthesia/Instrument/MIDINoteTriggerConfiguration")]
    public class MIDINoteTriggerConfiguration : TriggerConfiguration<MIDINote, 
        FloatMIDINoteDynamicExtractorSettings, FloatMIDINoteStaticExtractorSettings>
    {
        
    }
}