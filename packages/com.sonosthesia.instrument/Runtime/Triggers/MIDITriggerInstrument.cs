using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MIDITriggerInstrument : TriggerChannel<MIDINote>
    {
        [SerializeField] private TriggerValueGenerator<float> _channel;
        
        [SerializeField] private TriggerValueGenerator<float> _note;
        
        [SerializeField] private TriggerValueGenerator<float> _velocity;
        
        [SerializeField] private TriggerValueGenerator<float> _pressure;

        protected override bool Extract(Collider collider, out MIDINote midiNote)
        {
            midiNote = default;
            if (!_note.ProcessTriggerEnter(collider, out float note))
            {
                return false;
            }
            if (!_velocity.ProcessTriggerEnter(collider, out float velocity))
            {
                return false;
            }
            if (!_channel.ProcessTriggerEnter(collider, out float channel))
            {
                return false;
            }

            // pressure is optional
            float pressure = 0;
            if (_pressure && !_pressure.ProcessTriggerEnter(collider, out  pressure))
            {
                return false;
            }
            
            midiNote = new MIDINote((int)channel, (int)note, (int)velocity, (int)pressure);
            return true;
        }
    }
}