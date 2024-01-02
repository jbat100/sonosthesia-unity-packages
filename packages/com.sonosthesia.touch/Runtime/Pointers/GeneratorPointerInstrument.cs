using System;
using System.Collections.Generic;
using Sonosthesia.Channel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    public abstract class GeneratorPointerInstrument<TValue> : MonoBehaviour, 
        IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerExitHandler
        where TValue : struct
    {
        [SerializeField] private ChannelDriver<TValue> _driver;

        [SerializeField] private bool _endOnExit;
        
        private readonly Dictionary<int, Guid> _pointerEvents = new(); 

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId))
            {
                // unexpected but better end ongoing
                _driver.EndEvent(eventId);
                _pointerEvents.Remove(eventData.pointerId);
            }
            if (Extract(true, eventData, out TValue mpeNote))
            {
                _pointerEvents[eventData.pointerId] = _driver.BeginEvent(mpeNote);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId))
            {
                _driver.EndEvent(eventId);
                _pointerEvents.Remove(eventData.pointerId);
            }
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (_pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId) && Extract(false, eventData, out TValue value))
            {
                _driver.UpdateEvent(eventId, value);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_endOnExit && _pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId))
            {
                _driver.EndEvent(eventId);
                _pointerEvents.Remove(eventData.pointerId);
            }
        }

        protected abstract bool Extract(bool initial, PointerEventData eventData, out TValue value);
    }
}