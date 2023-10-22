using Sonosthesia.AdaptiveMIDI.Messages;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Instrument
{
    public class GeneratorMPEPointerInstrument : PolyphonicPointerInstrument<MPENote>
    {
        [SerializeField] private PointerValueGenerator<float> _note;
        
        [SerializeField] private PointerValueGenerator<float> _velocity;
        
        [SerializeField] private PointerValueGenerator<float> _pressure;
        
        [SerializeField] private PointerValueGenerator<float> _slide;
        
        [SerializeField] private PointerValueGenerator<float> _bend;
        
        protected override bool Extract(PointerEventData eventData, out MPENote mpeNote)
        {
            mpeNote = default;
            if (!_note.OnPointerDown(eventData, out float note))
            {
                return false;
            }
            if (!_velocity.OnPointerDown(eventData, out float velocity))
            {
                return false;
            }
            if (!_bend.OnPointerDown(eventData, out float bend))
            {
                return false;
            }
            if (!_slide.OnPointerDown(eventData, out float slide))
            {
                return false;
            }
            if (!_pressure.OnPointerDown(eventData, out float pressure))
            {
                return false;
            }

            mpeNote = new MPENote((int)note, (int)velocity, (int)slide, (int)pressure, bend);
            return true;
        }
    }
}