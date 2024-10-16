using System;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchTriggerAffordance : AbstractAffordance<TouchEvent>
    {
        [SerializeField] private Trigger.Trigger _trigger;

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
                TouchEnvelopeSession session = affordance._configuration.Settings
                    .SetupSession(e, out float _, affordance._trigger.TriggerController);
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return (_trigger && _configuration) ? new Controller(id, this) : null;
        }
    }
}