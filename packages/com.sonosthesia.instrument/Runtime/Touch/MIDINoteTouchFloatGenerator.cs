using Sonosthesia.MIDI;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MIDINoteTouchFloatGenerator : TouchValueGenerator<float>
    {
        [SerializeField] private MIDIPitch _note;
        
        public override bool BeginTouch(ITouchData touchData, out float value)
        {
            value = (int) _note;
            return true;
        }

        public override bool UpdateTouch(ITouchData touchData, out float value)
        {
            value = (int) _note;
            return true;
        }

        public override void EndTouch(ITouchData touchData)
        {
            
        }
    }
}