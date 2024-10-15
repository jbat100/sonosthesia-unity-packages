using System;
using System.Collections.Generic;
using UnityEngine;
using Sonosthesia.Envelope;
using Sonosthesia.Interaction;
using Sonosthesia.Trigger;

namespace Sonosthesia.Touch
{
    public class TouchTrackedTriggerAffordance : AbstractAffordance<TouchEvent>
    {
        [SerializeField] private List<TrackedTrigger> _triggers;

        [SerializeField] private TrackedTouchEnvelopeConfiguration _configuration;
        
        private class Controller : AffordanceController<TouchEvent, TouchTrackedTriggerAffordance>
        {
            private ITouchExtractorSession<float> _valueScaleSession;
            private readonly Guid _triggerId = Guid.NewGuid(); 
            
            public Controller(Guid eventId, TouchTrackedTriggerAffordance affordance) : base(eventId, affordance)
            {
                
            }

            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);

                TrackedTouchEnvelopeSettings settings = Affordance._configuration.Settings;

                _valueScaleSession = settings.ValueScaleExtractor.MakeSession(); 

                if (!_valueScaleSession.Setup(e, out float valueScale))
                {
                    valueScale = 1f;
                }
                if (!settings.TimeScaleExtractor.Extract(e, out float timeScale))
                {
                    timeScale = 1f;
                }
                
                IEnvelope envelope = settings.Envelope.Build();

                foreach (TrackedTrigger trigger in Affordance._triggers)
                {
                    trigger.StartTrigger(_triggerId, envelope, valueScale, timeScale);   
                }
            }

            protected override void Update(TouchEvent e)
            {
                base.Update(e);
                
                TrackedTouchEnvelopeSettings settings = Affordance._configuration.Settings;

                if (!settings.Track)
                {
                    return;
                }
                
                if (_valueScaleSession.Update(e, out float value))
                {
                    foreach (TrackedTrigger trigger in Affordance._triggers)
                    {
                        trigger.UpdateTrigger(_triggerId, value);   
                    }
                }
            }

            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                TrackedTouchEnvelopeSettings settings = Affordance._configuration.Settings;
                IEnvelope envelope = settings.ReleaseEnvelope(e);
                foreach (TrackedTrigger trigger in Affordance._triggers)
                {
                    trigger.EndTrigger(_triggerId, envelope);
                }
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return _configuration ? new Controller(id, this) : null;
        }
    }
}