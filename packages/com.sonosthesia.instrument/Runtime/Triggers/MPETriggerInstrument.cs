using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MPETriggerInstrument : TriggerChannelDriver<MPENote>
    {
        [SerializeField] private TriggerValueGenerator<float> _note;
        
        [SerializeField] private TriggerValueGenerator<float> _velocity;
        
        [SerializeField] private TriggerValueGenerator<float> _slide;
        
        [SerializeField] private TriggerValueGenerator<float> _pressure;
        
        [SerializeField] private TriggerValueGenerator<float> _bend;
        
        protected override bool Extract(bool initial, Collider collider, out MPENote mpeNote)
        {
            mpeNote = default;
            if (!_note.ProcessTrigger(initial, collider, out float note))
            {
                return false;
            }
            if (!_velocity.ProcessTrigger(initial, collider, out float velocity))
            {
                return false;
            }
            if (!_slide.ProcessTrigger(initial, collider, out float slide))
            {
                return false;
            }
            if (!_pressure.ProcessTrigger(initial, collider, out float pressure))
            {
                return false;
            }
            if (!_bend.ProcessTrigger(initial, collider, out float bend))
            {
                return false;
            }
            mpeNote = new MPENote((int)note, (int)velocity, (int)slide, (int)pressure, bend);
            return true;
        }
    }
}