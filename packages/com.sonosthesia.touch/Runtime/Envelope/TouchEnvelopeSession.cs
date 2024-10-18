using System;
using Sonosthesia.Envelope;
using Sonosthesia.Trigger;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public interface ITouchEnvelopeSession
    {
        void StartTouch(TouchEvent e);
        void UpdateTouch(TouchEvent e);
        void EndTouch(TouchEvent e, out float release);

        float Update();
    }
    
    public static class TouchEnvelopeSessionUtil
    {
        public static ITouchEnvelopeSession StartSession(TouchEvent e, 
            TouchEnvelopeSettings settings,
            TriggerController controller = null)
        {
            ITouchEnvelopeSession session = settings.Type switch
            {
                TouchEnvelopeSettings.TouchType.Constant => new ConstantTouchEnvelopeSession(settings, controller),
                TouchEnvelopeSettings.TouchType.Pulse => new PulseTouchEnvelopeSession(settings, controller),
                TouchEnvelopeSettings.TouchType.Contact => new ContactTouchEnvelopeSession(settings, controller),
                _ => throw new ArgumentOutOfRangeException()
            };
            session.StartTouch(e);
            return session;
        }

        private class TouchEnvelopeSession : ITouchEnvelopeSession
        {
            public virtual void StartTouch(TouchEvent e)
            {
                
            }

            public virtual void UpdateTouch(TouchEvent e)
            {
                
            }

            public virtual void EndTouch(TouchEvent e, out float release)
            {
                release = 0f;
            }

            public virtual float Update()
            {
                return 0;
            }
        }

        private class ConstantTouchEnvelopeSession : TouchEnvelopeSession
        {
            private readonly TouchEnvelopeSettings _settings;
            private readonly TriggerController _controller;
            private readonly Guid _triggerId = Guid.NewGuid();
            
            private float _constantValue;
            
            public ConstantTouchEnvelopeSession(TouchEnvelopeSettings settings, TriggerController controller)
            {
                _settings = settings;
                _controller = controller;
            }
            
            public override void StartTouch(TouchEvent e)
            {
                if (!_settings.ConstantExtractor.Extract(e, out _constantValue))
                {
                    _constantValue = 0f;
                }
                else
                {
                    Debug.Log($"{this} {nameof(StartTouch)} extracted {nameof(_constantValue)} {_constantValue}");
                }

                _controller?.StartTrigger(_triggerId, new ConstantEnvelope(_constantValue, float.PositiveInfinity), 1f, 1f);
            }

            public override void EndTouch(TouchEvent e, out float release)
            {
                release = 0;
                _controller?.EndTrigger(_triggerId, null, 0);
            }

            public override float Update()
            {
                // maintain value beyond trigger for affordances which use multiple envelopes
                return _constantValue;
            }
        }
        
        private class PulseTouchEnvelopeSession : TouchEnvelopeSession
        {
            private readonly TouchEnvelopeSettings _settings;
            private readonly TriggerController _controller;
            
            public PulseTouchEnvelopeSession(TouchEnvelopeSettings settings, TriggerController controller = null)
            {
                _settings = settings;
                _controller = controller ?? new TriggerController(AccumulationMode.Max);
            }
            
            public override void StartTouch(TouchEvent e)
            {
                IEnvelope envelope = _settings.Envelope.Build();

                if (!_settings.ValueScaleExtractor.Extract(e, out float valueScale))
                {
                    valueScale = 1f;
                }
                if (!_settings.TimeScaleExtractor.Extract(e, out float timeScale))
                {
                    timeScale = 1f;
                }

                _controller.PlayTrigger(envelope, valueScale, timeScale);
            }

            public override float Update()
            {
                return _controller.Update();
            }
        }
        
        private class ContactTouchEnvelopeSession : TouchEnvelopeSession
        {
            private readonly TouchEnvelopeSettings _settings;
            private readonly TriggerController _controller;
            private readonly Guid _triggerId = Guid.NewGuid();
            
            private ITouchExtractorSession<float> _valueScaleSession;

            public ContactTouchEnvelopeSession(TouchEnvelopeSettings settings, TriggerController controller = null)
            {
                _settings = settings;
                _controller = controller ?? new TriggerController(AccumulationMode.Max);
            }
            
            public override void StartTouch(TouchEvent e)
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

                _controller.StartTrigger(_triggerId, envelope, valueScale, timeScale);
            }

            public override void UpdateTouch(TouchEvent e)
            {
                if (!_settings.TrackValue)
                {
                    return;
                }
                if (_valueScaleSession?.Update(e, out float valueScale) ?? false)
                {
                    _controller.UpdateTrigger(_triggerId, valueScale);
                }
            }

            public override void EndTouch(TouchEvent e, out float release)
            {
                if (!_settings.ReleaseExtractor.Extract(e, out release))
                {
                    release = 1f;
                }
                IEnvelope envelope = _settings.ReleaseType.ReleaseEnvelope(release);
                _controller.EndTrigger(_triggerId, envelope);
            }

            public override float Update()
            {
                return _controller.Update();
            }
        }
    }
}