using Sonosthesia.Pointer;
using UnityEngine.EventSystems;

namespace Sonosthesia.Instrument
{
    public class MIDIPitchedPointerFloatGenerator : PointerValueGenerator<float>
    {
        private IMIDIPitchedElement _pitchedElement;

        protected virtual void Awake()
        {
            _pitchedElement = GetComponentInParent<IMIDIPitchedElement>();
        }

        private bool GetMIDIPitch(out float value)
        {
            if (_pitchedElement != null)
            {
                value = _pitchedElement.MIDINote;
                return true;    
            }

            value = default;
            return false;
        }
        
        public override bool OnPointerDown(PointerEventData eventData, out float value)
        {
            return GetMIDIPitch(out value);
        }

        public override bool OnPointerMove(PointerEventData eventData, out float value)
        {
            return GetMIDIPitch(out value);
        }

        public override void OnPointerEnd(PointerEventData eventData)
        {
            
        }
    }
}