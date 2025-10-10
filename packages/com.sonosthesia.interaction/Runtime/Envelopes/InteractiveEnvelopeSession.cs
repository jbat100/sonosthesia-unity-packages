using System;
using Sonosthesia.Envelope;
using Sonosthesia.Trigger;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public static class InteractiveEnvelopeSessionUtil
    {
        public static IInteractiveEnvelopeSession<TEvent> StartSession<TEvent>(TEvent e, 
            IInteractiveEnvelopeSettings<TEvent> settings,
            TriggerController controller = null) where TEvent : IInteractionEvent
        {
            IInteractiveEnvelopeSession<TEvent> session = InteractiveEnvelopeSessionUtil.MakeSession(settings, controller);
            session.Start(e);
            return session;
        }

        public static IInteractiveEnvelopeSession<TEvent> MakeSession<TEvent>(
            IInteractiveEnvelopeSettings<TEvent> settings,
            TriggerController controller) where TEvent : IInteractionEvent
        {
            IInteractiveEnvelopeSession<TEvent> session = settings.Interaction switch
            {
                EnvelopeInteraction.Constant => new ConstantEnvelopeSession<TEvent>(settings, controller),
                EnvelopeInteraction.Pulse => new PulseEnvelopeSession<TEvent>(settings, controller),
                EnvelopeInteraction.Contact => new ContactEnvelopeSession<TEvent>(settings, controller),
                _ => throw new ArgumentOutOfRangeException()
            };   
            if (settings.Filter == EnvelopeFilter.OneEuro)
            {
                session = new StaticInteractiveEnvelopeSessionOneEuroFilter<TEvent>(session, settings.OneEuroFilter);
            }

            return session;
        }
        
        private class EnvelopeSession<TEvent> : IInteractiveEnvelopeSession<TEvent> where TEvent : IInteractionEvent
        {
            protected readonly IInteractiveEnvelopeSettings<TEvent> Settings;
            protected readonly TriggerController Controller;
            protected readonly Guid TriggerId = Guid.NewGuid();
            
            public EnvelopeSession(IInteractiveEnvelopeSettings<TEvent> settings, TriggerController controller)
            {
                Settings = settings;
                Controller = controller ?? new TriggerController(AccumulationMode.Max);
            }
            
            public virtual void Start(TEvent e)
            {
                
            }

            public virtual void Update(TEvent e)
            {
                
            }

            public virtual void End(TEvent e, out float release)
            {
                release = 0f;
            }

            public virtual float Update()
            {
                return 0;
            }
        }

        private class ConstantEnvelopeSession<TEvent> : EnvelopeSession<TEvent> where TEvent : IInteractionEvent
        {
            private IExtractorSession<TEvent, float> _valueScaleSession;
            
            private float _valueScale;

            public ConstantEnvelopeSession(IInteractiveEnvelopeSettings<TEvent> settings, TriggerController controller) 
                : base(settings, controller)
            {
                
            }

            public override void Start(TEvent e)
            {
                _valueScaleSession = Settings.ConstantExtractor.MakeSession();
                
                if (!_valueScaleSession.Setup(e, out _valueScale))
                {
                    _valueScale = 0f;
                }

                Controller.StartTrigger(TriggerId, new ConstantEnvelope(1f, float.PositiveInfinity), _valueScale, 1f);
            }

            public override void Update(TEvent e)
            {
                if (!Settings.Track)
                {
                    return;
                }
                
                if (_valueScaleSession.Update(e, out _valueScale))
                {
                    Controller.UpdateTrigger(TriggerId, _valueScale);
                }
            }

            public override void End(TEvent e, out float release)
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
        
        private class PulseEnvelopeSession<TEvent> : EnvelopeSession<TEvent> where TEvent : IInteractionEvent
        {
            private float _endTime;
            
            public PulseEnvelopeSession(IInteractiveEnvelopeSettings<TEvent> settings, TriggerController controller) 
                : base(settings, controller)
            {
                
            }
            
            public override void Start(TEvent e)
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

                _endTime = Time.time + envelope.Duration * timeScale;

                Controller.PlayTrigger(envelope, valueScale, timeScale);
            }
            
            public override void End(TEvent e, out float release)
            {
                // make sure pulse has time to complete
                release = Mathf.Max(0, _endTime - Time.time);
            }

            public override float Update()
            {
                return Controller.Update();
            }
        }
        
        private class ContactEnvelopeSession<TEvent> : EnvelopeSession<TEvent> where TEvent : IInteractionEvent
        {
            private IExtractorSession<TEvent, float> _valueScaleSession;

            public ContactEnvelopeSession(IInteractiveEnvelopeSettings<TEvent> settings, TriggerController controller) 
                : base(settings, controller)
            {
                
            }
            
            public override void Start(TEvent e)
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

            public override void Update(TEvent e)
            {
                if (!Settings.Track)
                {
                    return;
                }
                if (_valueScaleSession.Update(e, out float valueScale))
                {
                    Controller.UpdateTrigger(TriggerId, valueScale);
                }
            }

            public override void End(TEvent e, out float release)
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