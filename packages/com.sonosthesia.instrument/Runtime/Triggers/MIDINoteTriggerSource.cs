using UnityEngine;
using Sonosthesia.MIDI;
using Sonosthesia.Touch;

namespace Sonosthesia.Instrument
{
    public class MIDINoteTriggerSource : TriggerSource<MIDINote>
    {
        [SerializeField] private TriggerValueGenerator<float> _channel;
        
        [SerializeField] private TriggerValueGenerator<float> _note;
        
        [SerializeField] private TriggerValueGenerator<float> _velocity;
        
        [SerializeField] private TriggerValueGenerator<float> _pressure;

        protected override bool Extract(bool initial, ITriggerData triggerData, out MIDINote midiNote)
        {
            midiNote = default;
            if (!_note.ProcessTrigger(initial, triggerData, out float note))
            {
                return false;
            }
            if (!_velocity.ProcessTrigger(initial, triggerData, out float velocity))
            {
                return false;
            }
            if (!_channel.ProcessTrigger(initial, triggerData, out float channel))
            {
                return false;
            }

            // pressure is optional
            float pressure = 0;
            if (_pressure && !_pressure.ProcessTrigger(initial, triggerData, out  pressure))
            {
                return false;
            }
            
            midiNote = new MIDINote((int)channel, (int)note, (int)velocity, (int)pressure);
            return true;
        }

        protected override void Clean(ITriggerData triggerData)
        {
            base.Clean(triggerData);
            _channel.EndTrigger(triggerData);
            _note.EndTrigger(triggerData);
            _velocity.EndTrigger(triggerData);
            _pressure.EndTrigger(triggerData);
        }
    }
}