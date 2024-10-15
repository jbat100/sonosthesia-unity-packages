using System;
using Sonosthesia.Envelope;
using Sonosthesia.Touch;
using Sonosthesia.Trigger;

namespace Sonosthesia.Touch
{
    // Uses an inner TriggerController to run the envelope, useful when we want to run 
    // touch envelopes dynamically, not tied to a GameObject Trigger

    public class TouchEnvelopeSession
    {
        private readonly TouchEnvelopeSettings _settings;
        private readonly TriggerController _triggerController = new (AccumulationMode.Max);
        
        public TouchEnvelopeSession(TouchEnvelopeSettings settings)
        {
            _settings = settings;
        }

        public void Setup(TouchEvent e, out float duration)
        {
            _settings.TriggerParameters(e, out IEnvelope envelope, out float valueScale, out float timeScale);
            _triggerController.StartTrigger(envelope, valueScale, timeScale);
            duration = envelope.Duration * timeScale;
        }

        public float Update()
        {
            return _triggerController.Update();
        }
    }
}