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

        [SerializeField] private TouchEnvelopeConfiguration _configuration;
        
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
                
                affordance._configuration.Settings
                    .TriggerParameters(e, out IEnvelope envelope, out float valueScale, out float timeScale);

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