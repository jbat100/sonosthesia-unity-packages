using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TriggerActor<TValue> : TriggerActorBase where TValue : struct
    {
        public readonly struct ValueState
        {
            public readonly Collider Actor;
            public readonly TValue Value;
            public readonly bool Colliding;

            public ValueState(Collider actor, TValue value, bool colliding)
            {
                Actor = actor;
                Value = value;
                Colliding = colliding;
            }
            
            public ValueState Push(TValue value, bool colliding)
            {
                return new ValueState(Actor, value, colliding);
            }
        }

        private readonly ReactiveDictionary<Guid, ValueState> _valueStates = new();
        public IReadOnlyReactiveDictionary<Guid, ValueState> ValueStates => _valueStates;

        private TriggerActorGroup<TValue> _group;

        protected virtual void Awake()
        {
            _group = GetComponentInParent<TriggerActorGroup<TValue>>();
        }
        
        protected virtual void OnEnable()
        {
            if (_group)
            {
                _group.RegisterValue(this);    
            }
        }
        
        protected virtual void OnDisable()
        {
            if (_group)
            {
                _group.UnregisterValue(this);    
            }
            _valueStates.Clear();
        }
        
        public override bool IsAvailable(Collider actor)
        {
            if (!base.IsAvailable(actor))
            {
                return false;
            }
            
            return !_group || _group.IsAvailable(actor);
        }
        
        public void OnTriggerStarted(Guid eventId, Collider actor, TValue value, bool colliding)
        {
            //Debug.Log($"{this} {nameof(OnTriggerStarted)} {eventId} {value}");
            _valueStates[eventId] = new ValueState(actor, value, colliding);
            OnStarted(eventId, actor, colliding);
        }
        
        public void OnTriggerUpdated(Guid eventId, Collider actor, TValue value, bool colliding)
        {
            //Debug.Log($"{this} {nameof(OnTriggerUpdated)} {eventId} {value}");
            _valueStates[eventId] = new ValueState(actor, value, colliding);
            OnUpdated(eventId, actor, colliding);
        }
        
        public void OnTriggerEnded(Guid eventId, Collider actor, bool colliding)
        {
            //Debug.Log($"{this} {nameof(OnTriggerEnded)} {eventId}");
            _valueStates.Remove(eventId);
            OnEnded(eventId, actor, colliding);
        }
        
    }
}