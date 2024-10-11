using System;
using System.Collections.Generic;
using Sonosthesia.Envelope;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchTriggerAffordance : AbstractAffordance<TouchEvent>
    {
        [SerializeField] private List<Trigger.Trigger> _triggers;

        [SerializeField] private TouchTriggerAffordanceConfiguration _configuration;
        
        private class Controller : AffordanceController<TouchEvent, TouchTriggerAffordance>
        {
            public Controller(Guid eventId, TouchTriggerAffordance affordance) : base(eventId, affordance)
            {
                
            }

            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);

                TouchTriggerAffordance affordance = Affordance;

                if (affordance._configuration == null)
                {
                    return;
                }
                
                ITouchExtractorSession<float> valueScaleSession = affordance._configuration.ValueScaleExtractor.MakeSession(); 
                ITouchExtractorSession<float> timeScaleSession = affordance._configuration.TimeScaleExtractor.MakeSession();
                IEnvelope envelope = affordance._configuration.Envelope.Build();

                if (!(valueScaleSession?.Setup(e, out float valueScale) ?? false))
                {
                    valueScale = 1f;
                }
                if (!(timeScaleSession?.Setup(e, out float timeScale) ?? false))
                {
                    timeScale = 1f;
                }

                foreach (Trigger.Trigger trigger in affordance._triggers)
                {
                    trigger.StartTrigger(envelope, valueScale, timeScale);
                }
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return new Controller(id, this);
        }
    }
}