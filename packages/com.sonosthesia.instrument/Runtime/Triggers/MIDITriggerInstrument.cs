using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MIDITriggerInstrument : TriggerChannelDriver<MIDINote>
    {
        [SerializeField] private TriggerValueGenerator<float> _channel;
        
        [SerializeField] private TriggerValueGenerator<float> _note;
        
        [SerializeField] private TriggerValueGenerator<float> _velocity;
        
        [SerializeField] private TriggerValueGenerator<float> _pressure;

        protected override bool Extract(bool initial, Collider collider, out MIDINote midiNote)
        {
            midiNote = default;
            if (!_note.ProcessTrigger(initial, collider, out float note))
            {
                return false;
            }
            if (!_velocity.ProcessTrigger(initial, collider, out float velocity))
            {
                return false;
            }
            if (!_channel.ProcessTrigger(initial, collider, out float channel))
            {
                return false;
            }

            // pressure is optional
            float pressure = 0;
            if (_pressure && !_pressure.ProcessTrigger(initial, collider, out  pressure))
            {
                return false;
            }
            
            midiNote = new MIDINote((int)channel, (int)note, (int)velocity, (int)pressure);
            return true;
        }
    }
}