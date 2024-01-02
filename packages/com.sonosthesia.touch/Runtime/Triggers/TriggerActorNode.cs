using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    // nodes form trees where each nodes gets trigger events from all descendants allowing 
    // arbitrary groups to be formed
    
    public class TriggerActorNode<TValue> : TriggerActorBase where TValue : struct
    {
        public readonly struct ValueState
        {
            public readonly TriggerChannelSource<TValue> Driver;
            public readonly Collider Actor;
            public readonly TValue Value;
            public readonly bool Colliding;

            public ValueState(TriggerChannelSource<TValue> driver, Collider actor, TValue value, bool colliding)
            {
                Driver = driver;
                Actor = actor;
                Value = value;
                Colliding = colliding;
            }
            
            public ValueState Push(TValue value, bool colliding)
            {
                return new ValueState(Driver, Actor, value, colliding);
            }
        }

        private readonly ReactiveDictionary<Guid, ValueState> _valueStates = new();
        public IReadOnlyReactiveDictionary<Guid, ValueState> ValueStates => _valueStates;

        [SerializeField] private TriggerActorNode<TValue> _parent;

        protected virtual void Awake()
        {
            if (_parent)
            {
                _parent = GetComponentInParent<TriggerActorNode<TValue>>();    
            }
        }

        protected virtual void OnDisable()
        {
            _valueStates.Clear();
        }
        
        public override bool IsAvailable(Collider actor)
        {
            if (!base.IsAvailable(actor))
            {
                return false;
            }
            
            return !_parent || _parent.IsAvailable(actor);
        }
        
        public void OnTriggerStarted(Guid eventId, TriggerChannelSource<TValue> driver, Collider actor, TValue value, bool colliding)
        {
            //Debug.Log($"{this} {nameof(OnTriggerStarted)} {eventId} {value}");

            if (_parent)
            {
                _parent.OnTriggerStarted(eventId, driver, actor, value, colliding);
            }
            
            _valueStates[eventId] = new ValueState(driver, actor, value, colliding);
            OnStarted(eventId, actor, colliding);
        }
        
        public void OnTriggerUpdated(Guid eventId, TriggerChannelSource<TValue> driver, Collider actor, TValue value, bool colliding)
        {
            //Debug.Log($"{this} {nameof(OnTriggerUpdated)} {eventId} {value}");
            
            if (_parent)
            {
                _parent.OnTriggerUpdated(eventId, driver, actor, value, colliding);
            }
            
            _valueStates[eventId] = new ValueState(driver, actor, value, colliding);
            OnUpdated(eventId, actor, colliding);
        }
        
        public void OnTriggerEnded(Guid eventId, TriggerChannelSource<TValue> driver, Collider actor, bool colliding)
        {
            //Debug.Log($"{this} {nameof(OnTriggerEnded)} {eventId}");
            
            if (_parent)
            {
                _parent.OnTriggerEnded(eventId, driver, actor, colliding);
            }
            
            _valueStates.Remove(eventId);
            OnEnded(eventId, actor, colliding);
        }
    }
}