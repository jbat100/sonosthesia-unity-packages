using System;
using System.Collections.Generic;
using Sonosthesia.Flow;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    public abstract class PointerChannelDriver<TValue> : ChannelDriver<TValue>, 
        IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerExitHandler
        where TValue : struct
    {
        [SerializeField] private bool _endOnExit;
        
        private readonly Dictionary<int, Guid> _pointerEvents = new(); 

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId))
            {
                // unexpected but better end ongoing
                EndEvent(eventId);
                _pointerEvents.Remove(eventData.pointerId);
            }
            if (Extract(true, eventData, out TValue mpeNote))
            {
                _pointerEvents[eventData.pointerId] = BeginEvent(mpeNote);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId))
            {
                EndEvent(eventId);
                _pointerEvents.Remove(eventData.pointerId);
            }
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (_pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId) && Extract(false, eventData, out TValue value))
            {
                UpdateEvent(eventId, value);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_endOnExit && _pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId))
            {
                EndEvent(eventId);
                _pointerEvents.Remove(eventData.pointerId);
            }
        }

        protected abstract bool Extract(bool initial, PointerEventData eventData, out TValue value);
    }
}