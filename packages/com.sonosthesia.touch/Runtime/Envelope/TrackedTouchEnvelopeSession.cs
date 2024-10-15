using System;
using Sonosthesia.Envelope;
using Sonosthesia.Trigger;

namespace Sonosthesia.Touch
{
    public class TrackedTouchEnvelopeSession
    {
        private readonly TrackedTouchEnvelopeSettings _settings;
        private readonly TrackedTriggerController _triggerController = new (AccumulationMode.Max);
        private readonly Guid _triggerId;
        
        private ITouchExtractorSession<float> _valueScaleSession;

        public TrackedTouchEnvelopeSession(TrackedTouchEnvelopeSettings settings)
        {
            _settings = settings;
        }
        
        public void StartTouch(TouchEvent e)
        {
            _valueScaleSession = _settings.ValueScaleExtractor.MakeSession(); 
            IEnvelope envelope = _settings.Envelope.Build();

            if (!_valueScaleSession.Setup(e, out float valueScale))
            {
                valueScale = 1f;
            }
            if (!_settings.TimeScaleExtractor.Extract(e, out float timeScale))
            {
                timeScale = 1f;
            }
            
            _triggerController.StartTrigger(_triggerId, envelope, valueScale, timeScale);
        }

        public void UpdateTouch(TouchEvent e)
        {
            if (!_settings.Track)
            {
                return;
            }
            
            if (_valueScaleSession?.Update(e, out float valueScale) ?? false)
            {
                _triggerController.UpdateTrigger(_triggerId, valueScale);
            }
        }

        public void EndTouch(TouchEvent e, out float duration)
        {
            IEnvelope envelope = _settings.ReleaseEnvelope(e);
            duration = envelope.Duration;
            _triggerController.EndTrigger(_triggerId, envelope);
        }

        public float Update()
        {
            return _triggerController.Update();
        }
    }
}