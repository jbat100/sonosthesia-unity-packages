using Sonosthesia.MIDI;
using Sonosthesia.Touch;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Instrument
{
    public class GeneratorMPEPointerInstrument : PointerSource<MPENote>
    {
        [SerializeField] private PointerValueGenerator<float> _note;
        
        [SerializeField] private PointerValueGenerator<float> _velocity;
        
        [SerializeField] private PointerValueGenerator<float> _pressure;
        
        [SerializeField] private PointerValueGenerator<float> _slide;
        
        [SerializeField] private PointerValueGenerator<float> _bend;
        
        protected override bool Extract(bool initial, PointerEventData eventData, out MPENote mpeNote)
        {
            mpeNote = default;
            
            if (!_note.OnPointer(initial, eventData, out float note))
            {
                return false;
            }
            if (!_velocity.OnPointer(initial, eventData, out float velocity))
            {
                return false;
            }
            if (!_bend.OnPointer(initial, eventData, out float bend))
            {
                return false;
            }
            if (!_slide.OnPointer(initial, eventData, out float slide))
            {
                return false;
            }
            if (!_pressure.OnPointer(initial, eventData, out float pressure))
            {
                return false;
            }

            mpeNote = new MPENote((int)note, (int)velocity, (int)slide, (int)pressure, bend);
            return true;
        }
    }
}