using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class TriggerAffordance<TEvent> : InteractionAffordance<TEvent> where TEvent : struct
    {
        [SerializeField] private Trigger.Trigger _trigger;

        [SerializeField] private InterfaceReference<ITriggerConfiguration<TEvent>>  _configuration;
        
        private class Controller : AffordanceController<TEvent, TriggerAffordance<TEvent>>
        {
            private IInteractiveTriggerSession<TEvent> _session; 
            
            public Controller(Guid eventId, TriggerAffordance<TEvent> affordance) : base(eventId, affordance)
            {
            }

            protected override void Setup(TEvent e)
            { 
                base.Setup(e);
                IInteractiveTriggerSettings<TEvent> settings = Affordance._configuration.Value.Settings;
                _session = settings.StartSession(e, Affordance._trigger.TriggerImplementation);
            }

            protected override void Update(TEvent e)
            {
                base.Update(e);
                _session.Update(e);
            }

            protected override void Teardown(TEvent e)
            {
                base.Teardown(e);
                _session.End(e, out float _);
            }
        }

        protected override IObserver<TEvent> MakeController(Guid id)
        {
            if (!_configuration)
            {
                this.LogError($"{this} missing configuration");
            }
            
            if (!_trigger)
            {
                this.LogError($"{this} missing trigger");
            }
            
            return (_configuration.Value != null && _trigger) ? new Controller(id, this) : null;
        }
    }
}