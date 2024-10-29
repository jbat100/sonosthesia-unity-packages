using System;
using UnityEngine;
using Sonosthesia.Interaction;

namespace Sonosthesia.Touch
{
    public class TouchTriggerAffordance : AbstractAffordance<TouchEvent>
    {
        [SerializeField] private Trigger.Trigger _trigger;

        [SerializeField] private TouchEnvelopeConfiguration _configuration;
        
        private class Controller : AffordanceController<TouchEvent, TouchTriggerAffordance>
        {
            private ITouchEnvelopeSession _session; 
            
            public Controller(Guid eventId, TouchTriggerAffordance affordance) : base(eventId, affordance)
            {
                
            }

            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);
                TouchEnvelopeSettings settings = Affordance._configuration.Settings;
                _session = settings.SetupSession(e, Affordance._trigger.TriggerController);
            }

            protected override void Update(TouchEvent e)
            {
                base.Update(e);
                _session.UpdateTouch(e);
            }

            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                _session.EndTouch(e, out float _);
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return (_configuration && _trigger) ? new Controller(id, this) : null;
        }
    }
}