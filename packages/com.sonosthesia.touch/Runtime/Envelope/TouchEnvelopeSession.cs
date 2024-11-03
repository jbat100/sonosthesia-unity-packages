using System;
using Sonosthesia.Envelope;
using Sonosthesia.Trigger;

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
            protected readonly TouchEnvelopeSettings Settings;
            protected readonly TriggerController Controller;
            protected readonly Guid TriggerId = Guid.NewGuid();
            
            public TouchEnvelopeSession(TouchEnvelopeSettings settings, TriggerController controller)
            {
                Settings = settings;
                Controller = controller ?? new TriggerController(AccumulationMode.Max);
            }

            
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
            private ITouchExtractorSession<float> _valueScaleSession;
            
            private float _valueScale;

            public ConstantTouchEnvelopeSession(TouchEnvelopeSettings settings, TriggerController controller) 
                : base(settings, controller)
            {
                
            }

            public override void StartTouch(TouchEvent e)
            {
                _valueScaleSession = Settings.ConstantExtractor.MakeSession();
                
                if (!_valueScaleSession.Setup(e, out _valueScale))
                {
                    _valueScale = 0f;
                }

                Controller.StartTrigger(TriggerId, new ConstantEnvelope(1f, float.PositiveInfinity), _valueScale, 1f);
            }

            public override void UpdateTouch(TouchEvent e)
            {
                if (!Settings.TrackValue)
                {
                    return;
                }
                
                if (_valueScaleSession.Update(e, out _valueScale))
                {
                    Controller.UpdateTrigger(TriggerId, _valueScale);
                }
            }

            public override void EndTouch(TouchEvent e, out float release)
            {
                release = 0;
                Controller.EndTrigger(TriggerId, null, 0);
            }

            public override float Update()
            {
                // maintain value beyond trigger for affordances which use multiple envelopes
                return _valueScale;
            }
        }
        
        private class PulseTouchEnvelopeSession : TouchEnvelopeSession
        {
            public PulseTouchEnvelopeSession(TouchEnvelopeSettings settings, TriggerController controller) 
                : base(settings, controller)
            {
                
            }
            
            public override void StartTouch(TouchEvent e)
            {
                IEnvelope envelope = Settings.Envelope.Build();

                if (!Settings.ValueScaleExtractor.Extract(e, out float valueScale))
                {
                    valueScale = 1f;
                }
                if (!Settings.TimeScaleExtractor.Extract(e, out float timeScale))
                {
                    timeScale = 1f;
                }

                Controller.PlayTrigger(envelope, valueScale, timeScale);
            }

            public override float Update()
            {
                return Controller.Update();
            }
        }
        
        private class ContactTouchEnvelopeSession : TouchEnvelopeSession
        {
            private ITouchExtractorSession<float> _valueScaleSession;

            public ContactTouchEnvelopeSession(TouchEnvelopeSettings settings, TriggerController controller) 
                : base(settings, controller)
            {
                
            }
            
            public override void StartTouch(TouchEvent e)
            {
                _valueScaleSession = Settings.ValueScaleExtractor.MakeSession(); 
                IEnvelope envelope = Settings.Envelope.Build();

                if (!_valueScaleSession.Setup(e, out float valueScale))
                {
                    valueScale = 1f;
                }
                if (!Settings.TimeScaleExtractor.Extract(e, out float timeScale))
                {
                    timeScale = 1f;
                }

                Controller.StartTrigger(TriggerId, envelope, valueScale, timeScale);
            }

            public override void UpdateTouch(TouchEvent e)
            {
                if (!Settings.TrackValue)
                {
                    return;
                }
                if (_valueScaleSession.Update(e, out float valueScale))
                {
                    Controller.UpdateTrigger(TriggerId, valueScale);
                }
            }

            public override void EndTouch(TouchEvent e, out float release)
            {
                if (!Settings.ReleaseExtractor.Extract(e, out release))
                {
                    release = 1f;
                }
                IEnvelope envelope = Settings.ReleaseType.ReleaseEnvelope(release);
                Controller.EndTrigger(TriggerId, envelope);
            }

            public override float Update()
            {
                return Controller.Update();
            }
        }
    }
}