using UnityEngine;
using UnityEngine.EventSystems;
using Sonosthesia.MIDI;
using Sonosthesia.Touch;

namespace Sonosthesia.Instrument
{
    public class GeneratorMIDIPointerInstrument : GeneratorPointerInstrument<MIDINote>
    {
        [SerializeField] private PointerValueGenerator<float> _channel;
        
        [SerializeField] private PointerValueGenerator<float> _note;
        
        [SerializeField] private PointerValueGenerator<float> _velocity;
        
        [SerializeField] private PointerValueGenerator<float> _pressure;

        protected override bool Extract(bool initial, PointerEventData eventData, out MIDINote midiNote)
        {
            midiNote = default;
            if (!_note.OnPointer(initial, eventData, out float note))
            {
                return false;
            }
            if (!_velocity.OnPointer(initial, eventData, out float velocity))
            {
                return false;
            }
            if (!_channel.OnPointer(initial, eventData, out float channel))
            {
                return false;
            }

            // pressure is optional
            float pressure = 0;
            if (_pressure && !_pressure.OnPointer(initial, eventData, out  pressure))
            {
                return false;
            }
            
            midiNote = new MIDINote((int)channel, (int)note, (int)velocity, (int)pressure);
            return true;
        }
    }
}