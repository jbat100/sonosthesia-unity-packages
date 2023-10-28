using System;
using System.Collections.Generic;
using Sonosthesia.Channel;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TriggerChannelDriver<TValue> : ChannelDriver<TValue> where TValue : struct
    {
        [SerializeField] private bool _endOnExit = true;
        
        [SerializeField] private bool _restartOnEnter = true;

        // note : we don't want concurrent events from the same collider
        
        private readonly HashSet<Collider> _colliding = new();

        private class Trigger
        {
            public readonly Guid EventId;
            public readonly TriggerActor<TValue> Actor;

            public Trigger(Guid eventId, TriggerActor<TValue> actor)
            {
                EventId = eventId;
                Actor = actor;
            }
        }

        private readonly Dictionary<Collider, Trigger> _triggers = new();
        
        public void EndEvent(Collider other)
        {
            EndEvent(other, _colliding.Contains(other));
        }

        protected virtual void FixedUpdate()
        {
            foreach (KeyValuePair<Collider, Trigger> pair in _triggers)
            {
                if (_colliding.Contains(pair.Key))
                {
                    continue;
                }
                UpdateEvent(pair.Key, pair.Value, false);
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            //Debug.Log($"{this} {nameof(OnTriggerEnter)} {other}");
            
            _colliding.Add(other);
            
            if (_triggers.TryGetValue(other, out Trigger trigger))
            {
                if (!_restartOnEnter)
                {
                    UpdateEvent(other, trigger, true);
                    return;
                }
                EndEvent(other, true);
            }
            
            TriggerActor<TValue> actor = other.GetComponentInParent<TriggerActor<TValue>>();
            if (actor && !actor.IsAvailable(other))
            {
                return;
            }
            
            if (Extract(true, other, out TValue value))
            {
                Guid evendId = BeginEvent(value);
                _triggers[other] = new Trigger(evendId, actor);
                if (actor)
                {
                    actor.OnTriggerStarted(evendId, this, other, value, true);
                }
            }
        }
        
        protected virtual void OnTriggerStay(Collider other)
        {
            //Debug.Log($"{this} {nameof(OnTriggerStay)} {other}");
            
            if (_triggers.TryGetValue(other, out Trigger trigger))
            {
                UpdateEvent(other, trigger, true);
            }
        }
        
        protected virtual void OnTriggerExit(Collider other)
        {
            //Debug.Log($"{this} {nameof(OnTriggerExit)} {other}");
            
            _colliding.Remove(other);
            
            if (_endOnExit)
            {
                EndEvent(other, false);
            }
        }

        private void UpdateEvent(Collider other, Trigger trigger, bool colliding)
        {
            if (Extract(false, other, out TValue value))
            {
                UpdateEvent(trigger.EventId, value);
                if (trigger.Actor)
                {
                    trigger.Actor.OnTriggerUpdated(trigger.EventId, this, other, value, colliding);
                }
            }
        }

        private void EndEvent(Collider other, bool colliding)
        {
            if (_triggers.TryGetValue(other, out Trigger trigger))
            {
                _triggers.Remove(other);
                EndEvent(trigger.EventId);
                if (trigger.Actor)
                {
                    trigger.Actor.OnTriggerEnded(trigger.EventId, this, other, colliding);
                }
            }
        }
        
        protected abstract bool Extract(bool initial, Collider collider, out TValue value);
    }
}