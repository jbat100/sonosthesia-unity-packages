using System;
using System.Collections.Generic;
using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TriggerChannel<TValue> : Channel<TValue> where TValue : struct
    {
        [SerializeField] private bool _endOnExit = true;

        private readonly HashSet<Collider> _colliding = new();

        private readonly Dictionary<Collider, Guid> _triggerEvents = new();

        public void EndEvent(Collider other)
        {
            if (_triggerEvents.TryGetValue(other, out Guid eventId))
            {
                EndEvent(eventId);
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_triggerEvents.TryGetValue(other, out Guid eventId))
            {
                EndEvent(eventId);
                _triggerEvents.Remove(other);
            }
            if (Extract(other, out TValue value))
            {
                _triggerEvents[other] = BeginEvent(value);
            }
        }
        
        protected virtual void OnTriggerStay(Collider other)
        {
            if (_triggerEvents.TryGetValue(other, out Guid eventId) && Extract(other, out TValue value))
            {
                UpdateEvent(eventId, value);
            }
        }
        
        protected virtual void OnTriggerExit(Collider other)
        {
            if (_endOnExit && _triggerEvents.TryGetValue(other, out Guid eventId))
            {
                EndEvent(eventId);
            }
        }
        
        protected abstract bool Extract(Collider collider, out TValue value);
    }
}