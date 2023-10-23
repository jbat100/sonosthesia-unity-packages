using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MIDIPitchedTriggerFloatGenerator : TriggerValueGenerator<float>
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

        public override bool ProcessTriggerEnter(Collider other, out float value)
        {
            return GetMIDIPitch(out value);
        }

        public override bool ProcessTriggerStay(Collider other, out float value)
        {
            return GetMIDIPitch(out value);
        }

        public override void ProcessTriggerExit(Collider other)
        {
            
        }
    }
}