using Sonosthesia.MIDI;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MIDINoteNameTriggerFloatGenerator : TriggerValueGenerator<float>
    {
        [SerializeField] [Range(-1, 9)] private int _octave;

        [SerializeField] private MIDINoteName _note;
        
        public override bool BeginTrigger(ITriggerData triggerData, out float value)
        {
            if (_note.MIDIPitchForOctave(_octave, out MIDIPitch result))
            {
                value = (int) result;
                return true;
            }
            value = 0f;
            return false;
        }

        public override bool UpdateTrigger(ITriggerData triggerData, out float value)
        {
            if (_note.MIDIPitchForOctave(_octave, out MIDIPitch result))
            {
                value = (int) result;
                return true;
            }
            value = 0f;
            return false;
        }

        public override void EndTrigger(ITriggerData triggerData)
        {
            throw new System.NotImplementedException();
        }
    }
}