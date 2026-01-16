using Sonosthesia.Interaction;
using Sonosthesia.MIDI;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [CreateAssetMenu(fileName = "MPENoteTriggerConfiguration", 
        menuName = "Sonosthesia/Instrument/MPENoteTriggerConfiguration")]
    public class MPENoteTriggerConfiguration : TriggerConfiguration<MPENote, 
        FloatMPENoteDynamicExtractorSettings, FloatMPENoteStaticExtractorSettings>
    {
        
    }
}