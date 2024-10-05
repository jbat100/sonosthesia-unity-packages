using System;
using UnityEngine;
using Sonosthesia.MIDI;
using Sonosthesia.Touch;

namespace Sonosthesia.Instrument
{
    public class MIDINoteTouchSource : ValueTouchSource<MIDINote>
    {
        [SerializeField] private TouchValueGenerator<float> _channel;
        
        [SerializeField] private TouchValueGenerator<float> _note;
        
        [SerializeField] private TouchValueGenerator<float> _velocity;
        
        [SerializeField] private TouchValueGenerator<float> _pressure;

        protected override bool Extract(bool initial, ITouchData touchData, out MIDINote midiNote)
        {
            midiNote = default;
            if (!_note.ProcessTrigger(initial, touchData, out float note))
            {
                return false;
            }
            if (!_velocity.ProcessTrigger(initial, touchData, out float velocity))
            {
                return false;
            }
            if (!_channel.ProcessTrigger(initial, touchData, out float channel))
            {
                return false;
            }

            // pressure is optional
            float pressure = 0;
            if (_pressure && !_pressure.ProcessTrigger(initial, touchData, out  pressure))
            {
                return false;
            }
            
            midiNote = new MIDINote((int)channel, (int)note, (int)velocity, (int)pressure);
            return true;
        }

        protected override void CleanupStream(Guid eventId, ITouchData touchData)
        {
            base.CleanupStream(eventId, touchData);
            _channel.EndTouch(touchData);
            _note.EndTouch(touchData);
            _velocity.EndTouch(touchData);
            _pressure.EndTouch(touchData);
        }
    }
}