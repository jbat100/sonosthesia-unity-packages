using System;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class AffordanceController<TEvent, TAffordance>  : IObserver<TEvent> 
        where TEvent : struct, IInteractionEvent
        where TAffordance : AbstractAffordance<TEvent>
    {
        private readonly TAffordance _affordance;
        protected TAffordance Affordance => _affordance;
        
        private readonly Guid _eventId;
        protected Guid EventId => _eventId;
        
        private bool _initialized;
        private bool _blocked;
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
                Debug.Log($"{_affordance} controller {nameof(Setup)} {e}");
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
                Debug.Log($"{_affordance} controller {nameof(Teardown)} {e}");
            }
        }

        protected virtual bool CheckCompatibility(TEvent e)
        {
            if ((e.Source.InteractionLayers & _affordance.SourceInteractionLayers) == 0)
            {
                return false;
            }
            if ((e.Actor.InteractionLayers & _affordance.ActorInteractionLayers) == 0)
            {
                return false;
            }

            return true;
        }

        public void OnNext(TEvent e)
        {
            if (_blocked)
            {
                return;
            }
            
            _latest = e;

            if (!_initialized)
            {
                _initialized = true;
                if (!CheckCompatibility(e))
                {
                    _blocked = true;
                    return;
                }
                Setup(e);
                return;
            }

            Update(e);
        }
        
        public void OnCompleted()
        {
            if (!_blocked)
            {
                Teardown(_latest);    
            }
        }

        public void OnError(Exception error)
        {
            if (!_blocked)
            {
                Teardown(_latest);    
            }
        }
    }
}