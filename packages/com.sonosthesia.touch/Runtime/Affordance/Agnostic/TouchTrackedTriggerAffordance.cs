using System;
using UnityEngine;
using Sonosthesia.Envelope;
using Sonosthesia.Interaction;
using Sonosthesia.Trigger;

namespace Sonosthesia.Touch
{
    public class TouchTrackedTriggerAffordance : AgnosticAffordance<TouchEvent, TouchTrackedTriggerAffordance>
    {
        [SerializeField] private TrackedTrigger _trigger;

        [SerializeField] private bool _track;

        [SerializeField] private TouchTrackedTriggerAffordanceConfiguration _configuration;
        
        protected new class Controller : AgnosticAffordance<TouchEvent, TouchTrackedTriggerAffordance>.Controller
        {
            private Guid _triggerId;
            private ITouchExtractorSession<float> _valueScaleSession;
            
            public Controller(Guid eventId, TouchTrackedTriggerAffordance affordance) : base(eventId, affordance)
            {
                
            }

            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);

                TouchTrackedTriggerAffordance affordance = Affordance;
                
                if (!affordance._configuration)
                {
                    _triggerId = affordance._trigger.StartTrigger(1f, 1f);
                    return;
                }

                _valueScaleSession = affordance._configuration.StartValueScaleExtractor.MakeSession(); 
                ITouchExtractorSession<float> timeScaleSession = affordance._configuration.StartTimeScaleExtractor.MakeSession();
                IEnvelope envelope = affordance._configuration.StartEnvelope?.Build();

                if (!(_valueScaleSession?.Setup(e, out float valueScale) ?? false))
                {
                    valueScale = 1f;
                }
                if (!(timeScaleSession?.Setup(e, out float timeScale) ?? false))
                {
                    timeScale = 1f;
                }
                _triggerId = affordance._trigger.StartTrigger(envelope, valueScale, timeScale);
            }

            protected override void Update(TouchEvent e)
            {
                base.Update(e);
                
                TouchTrackedTriggerAffordance affordance = Affordance;

                if (!affordance._track)
                {
                    return;
                }

                if (_valueScaleSession == null)
                {
                    return;
                }

                if (!_valueScaleSession.Update(e, out float value))
                {
                    return;
                }

                Affordance._trigger.UpdateTrigger(_triggerId, value);
            }

            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                
                TouchTrackedTriggerAffordance affordance = Affordance;

                if (!affordance._configuration)
                {
                    affordance._trigger.EndTrigger(_triggerId);
                    return;
                }
                
                ITouchExtractorSession<float> timeScaleSession = affordance._configuration.EndTimeScaleExtractor.MakeSession();
                if (!(timeScaleSession?.Setup(e, out float timeScale) ?? false))
                {
                    timeScale = 1f;
                }
                IEnvelope envelope = affordance._configuration.EndEnvelope?.Build();
                affordance._trigger.EndTrigger(_triggerId, envelope, timeScale);
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return new Controller(id, this);
        }
    }
}