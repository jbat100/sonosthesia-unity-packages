using System;
using System.Collections.Generic;
using Sonosthesia.Channel;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TriggerChannelDriver<TValue> : ChannelDriver<TValue> where TValue : struct
    {
        [SerializeField] private bool _endOnExit = true;

        private readonly HashSet<Collider> _colliding = new();

        private readonly Dictionary<Collider, Guid> _triggerEvents = new();

        public void EndEvent(Collider other)
        {
            if (_triggerEvents.TryGetValue(other, out Guid eventId))
            {
                _triggerEvents.Remove(other);
                EndEvent(eventId);
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            Debug.Log($"{this} {nameof(OnTriggerEnter)} {other}");
            if (_triggerEvents.TryGetValue(other, out Guid eventId))
            {
                _triggerEvents.Remove(other);
                EndEvent(eventId);
            }
            if (Extract(true, other, out TValue value))
            {
                _triggerEvents[other] = BeginEvent(value);
            }
        }
        
        protected virtual void OnTriggerStay(Collider other)
        {
            Debug.Log($"{this} {nameof(OnTriggerStay)} {other}");
            if (_triggerEvents.TryGetValue(other, out Guid eventId) && Extract(false, other, out TValue value))
            {
                UpdateEvent(eventId, value);
            }
        }
        
        protected virtual void OnTriggerExit(Collider other)
        {
            Debug.Log($"{this} {nameof(OnTriggerExit)} {other}");
            if (_endOnExit && _triggerEvents.TryGetValue(other, out Guid eventId))
            {
                _triggerEvents.Remove(other);
                EndEvent(eventId);
            }
        }
        
        protected abstract bool Extract(bool initial, Collider collider, out TValue value);
    }
}