using Sonosthesia.Envelope;
using Sonosthesia.Trigger;

namespace Sonosthesia.Touch
{
    // Uses an inner TriggerController to run the envelope, useful when we want to run 
    // touch envelopes dynamically, not tied to a GameObject Trigger

    public class TouchEnvelopeSession
    {
        private readonly TouchEnvelopeSettings _settings;
        private readonly TriggerController _controller;
        
        public TouchEnvelopeSession(TouchEnvelopeSettings settings, TriggerController controller = null)
        {
            _settings = settings;
            _controller = controller ?? new TriggerController(AccumulationMode.Max);
        }

        public void Setup(TouchEvent e, out float duration)
        {
            ITouchExtractorSession<float> valueScaleSession = _settings.ValueScaleExtractor.MakeSession(); 
            ITouchExtractorSession<float> timeScaleSession = _settings.TimeScaleExtractor.MakeSession();
            IEnvelope envelope = _settings.Envelope.Build();

            if (!(valueScaleSession?.Setup(e, out float valueScale) ?? false))
            {
                valueScale = 1f;
            }
            if (!(timeScaleSession?.Setup(e, out float timeScale) ?? false))
            {
                timeScale = 1f;
            }
            
            _controller.PlayTrigger(envelope, valueScale, timeScale);
            duration = envelope.Duration * timeScale;
        }

        public float Update()
        {
            return _controller.Update();
        }
    }
}