using Sonosthesia.MIDI;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MPENoteTriggerChannelSource : TriggerChannelSource<MPENote>
    {
        [SerializeField] private TriggerValueGenerator<float> _note;
        
        [SerializeField] private TriggerValueGenerator<float> _velocity;
        
        [SerializeField] private TriggerValueGenerator<float> _slide;
        
        [SerializeField] private TriggerValueGenerator<float> _pressure;
        
        [SerializeField] private TriggerValueGenerator<float> _bend;
        
        protected override bool Extract(bool initial, ITriggerData triggerData, out MPENote mpeNote)
        {
            mpeNote = default;
            if (!_note.ProcessTrigger(initial, triggerData, out float note))
            {
                return false;
            }
            if (!_velocity.ProcessTrigger(initial, triggerData, out float velocity))
            {
                return false;
            }
            if (!_slide.ProcessTrigger(initial, triggerData, out float slide))
            {
                return false;
            }
            if (!_pressure.ProcessTrigger(initial, triggerData, out float pressure))
            {
                return false;
            }
            if (!_bend.ProcessTrigger(initial, triggerData, out float bend))
            {
                return false;
            }
            mpeNote = new MPENote((int)note, (int)velocity, (int)slide, (int)pressure, bend);
            return true;
        }
        
        protected override void Clean(ITriggerData triggerData)
        {
            base.Clean(triggerData);
            _note.EndTrigger(triggerData);
            _velocity.EndTrigger(triggerData);
            _pressure.EndTrigger(triggerData);
            _slide.EndTrigger(triggerData);
            _bend.EndTrigger(triggerData);
        }
    }
}