using Sonosthesia.MIDI;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MIDINoteTriggerFloatGenerator : TriggerValueGenerator<float>
    {
        [SerializeField] private MIDIPitch _note;
        
        public override bool BeginTrigger(ITriggerData triggerData, out float value)
        {
            value = (int) _note;
            return true;
        }

        public override bool UpdateTrigger(ITriggerData triggerData, out float value)
        {
            value = (int) _note;
            return true;
        }

        public override void EndTrigger(ITriggerData triggerData)
        {
            
        }
    }
}