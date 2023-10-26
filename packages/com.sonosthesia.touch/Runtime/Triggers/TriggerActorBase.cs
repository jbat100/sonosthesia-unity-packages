using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TriggerActorBase : MonoBehaviour
    {
        public readonly struct State
        {
            public readonly Collider Actor;
            public readonly bool Colliding;

            public State(Collider actor, bool colliding)
            {
                Actor = actor;
                Colliding = colliding;
            }
            
            public State Push(bool colliding)
            {
                return new State(Actor, colliding);
            }
        }

        [SerializeField] private int _maxConcurrent = 1;

        private readonly ReactiveDictionary<Guid, State> _states = new();
        public IReadOnlyReactiveDictionary<Guid, State> States => _states;

        public virtual bool IsAvailable(Collider other)
        {
            if (!isActiveAndEnabled)
            {
                return false;
            }
            
            return _states.Count < _maxConcurrent;
        }
        
        protected void OnStarted(Guid eventId, Collider actor, bool colliding)
        {
            _states[eventId] = new State(actor, colliding);
        }
        
        protected void OnUpdated(Guid eventId, Collider actor, bool colliding)
        {
            _states[eventId] = new State(actor, colliding);
        }

        protected void OnEnded(Guid eventId, Collider actor, bool colliding)
        {
            _states.Remove(eventId);
        }
    }
}