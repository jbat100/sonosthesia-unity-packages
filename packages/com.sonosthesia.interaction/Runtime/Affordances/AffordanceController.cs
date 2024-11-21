using System;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class AffordanceController<TEvent, TAffordance>  : IObserver<TEvent> 
        where TEvent : struct, IInteractionEvent
        where TAffordance : InteractionAffordance<TEvent>
    {
        private readonly TAffordance _affordance;
        protected TAffordance Affordance => _affordance;
        
        private readonly Guid _eventId;
        protected Guid EventId => _eventId;
        
        private bool _initialized;
        private TEvent _latest;

        public AffordanceController(Guid eventId, TAffordance affordance)
        {
            _eventId = eventId;
            _affordance = affordance;
        }

        protected virtual void Setup(TEvent e)
        {
            if (_affordance.Log)
            {
                Debug.LogWarning($"{_affordance} controller {nameof(Setup)} {EventId} {e}");
            }
        }

        protected virtual void Update(TEvent e)
        {
            if (_affordance.Log)
            {
                // Debug.Log($"{_affordance} controller {nameof(Update)} {e}");
            }
        }

        protected virtual void Teardown(TEvent e)
        {
            if (_affordance.Log)
            {
                Debug.LogWarning($"{_affordance} controller {nameof(Teardown)} {EventId} {e}");
            }
        }
        
        public void OnNext(TEvent e)
        {
            _latest = e;

            if (!_initialized)
            {
                _initialized = true;
                Setup(e);
                return;
            }

            Update(e);
        }
        
        public void OnCompleted()
        {
            Teardown(_latest);
        }

        public void OnError(Exception error)
        {
            Teardown(_latest);
        }
    }
}