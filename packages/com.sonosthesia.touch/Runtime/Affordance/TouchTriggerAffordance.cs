using System;
using UnityEngine;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;

namespace Sonosthesia.Touch
{
    public class TouchTriggerAffordance : InteractionAffordance<TouchEvent>
    {
        [SerializeField] private Trigger.Trigger _trigger;

        [SerializeField] private TouchEnvelopeConfiguration _configuration;
        
        // TODO : scriptable object for indicator, takes _configuration as argument with extra info, key, color, offset
        
        private class Controller : AffordanceController<TouchEvent, TouchTriggerAffordance>
        {
            private IInteractiveEnvelopeSession<TouchEvent> _session; 
            
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
                _session.Update(e);
            }

            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                _session.End(e, out float _);
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            if (!_configuration)
            {
                this.LogError($"{this} missing configuration");
            }

            if (!_trigger)
            {
                this.LogError($"{this} missing trigger");
            }
            
            return (_configuration && _trigger) ? new Controller(id, this) : null;
        }
    }
}