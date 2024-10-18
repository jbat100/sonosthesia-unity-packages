using System;
using Sonosthesia.Envelope;
using UnityEngine;
using UnityEngine.UI;

namespace Sonosthesia 
{
    public class TriggerHoldUI : MonoBehaviour
    {
        [SerializeField] private Slider _valueScaleSlider;
        
        [SerializeField] private Trigger.Trigger _trigger;

        [SerializeField] private EnvelopeFactory _playEnvelope;
        
        [SerializeField] private EnvelopeFactory _startEnvelope;
        
        [SerializeField] private EnvelopeFactory _endEnvelope;

        private Guid _holdId;
        
        public void Trigger()
        {
            Debug.Log($"{this} {nameof(Trigger)}");
            if (!_trigger)
            {
                return;
            }
            float valueScale = _valueScaleSlider ? _valueScaleSlider.value : 1f;
            IEnvelope envelope = _playEnvelope ? _playEnvelope.Build() : null;
            _trigger.TriggerController.PlayTrigger(envelope, valueScale, 1f);
        }

        public void StartHold()
        {
            Debug.Log($"{this} {nameof(StartHold)}");
            if (!_trigger)
            {
                return;
            }
            EndHold();
            float valueScale = _valueScaleSlider ? _valueScaleSlider.value : 1f;
            IEnvelope envelope = _startEnvelope ? _startEnvelope.Build() : null;
            _holdId = _trigger.TriggerController.StartTrigger(envelope, valueScale, 1f);
        }

        public void EndHold()
        {
            if (!_trigger || _holdId == Guid.Empty)
            {
                return;
            }
            Debug.Log($"{this} {nameof(EndHold)}");
            IEnvelope envelope = _endEnvelope ? _endEnvelope.Build() : null;
            _trigger.TriggerController.EndTrigger(_holdId, envelope);
            _holdId = Guid.Empty;
        }
    }

}


