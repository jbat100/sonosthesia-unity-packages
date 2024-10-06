using System;
using UnityEngine;
using Sonosthesia.MIDI;
using Sonosthesia.Touch;

namespace Sonosthesia.Instrument
{
    public class MPENoteTouchSource : ValueTouchSource<MPENote>
    {
        [SerializeField] private TouchValueGenerator<float> _note;
        
        [SerializeField] private TouchValueGenerator<float> _velocity;
        
        [SerializeField] private TouchValueGenerator<float> _slide;
        
        [SerializeField] private TouchValueGenerator<float> _pressure;
        
        [SerializeField] private TouchValueGenerator<float> _bend;
        
        protected override bool Extract(bool initial, ITouchData touchData, out MPENote mpeNote)
        {
            mpeNote = default;
            if (!_note.ProcessTrigger(initial, touchData, out float note))
            {
                return false;
            }
            if (!_velocity.ProcessTrigger(initial, touchData, out float velocity))
            {
                return false;
            }
            if (!_slide.ProcessTrigger(initial, touchData, out float slide))
            {
                return false;
            }
            if (!_pressure.ProcessTrigger(initial, touchData, out float pressure))
            {
                return false;
            }
            if (!_bend.ProcessTrigger(initial, touchData, out float bend))
            {
                return false;
            }
            mpeNote = new MPENote((int)note, (int)velocity, (int)slide, (int)pressure, bend);
            return true;
        }
        
        protected override void CleanupStream(Guid id, ITouchData touchData)
        {
            base.CleanupStream(id, touchData);
            _note.EndTouch(touchData);
            _velocity.EndTouch(touchData);
            _pressure.EndTouch(touchData);
            _slide.EndTouch(touchData);
            _bend.EndTouch(touchData);
        }
    }
}