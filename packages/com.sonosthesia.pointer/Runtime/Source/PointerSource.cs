using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Sonosthesia.Channel;

namespace Sonosthesia.Pointer
{
    public abstract class PointerSource<TValue> : BasePointerSource, 
        IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerExitHandler,
        IScrollHandler,
        IDragHandler, IInitializePotentialDragHandler
        where TValue : struct
    {
        private enum TrackingMode
        {
            Move,
            Drag
        }

        [SerializeField] private ChannelDriver<TValue> _driver;

        [SerializeField] private PointerValueEventStreamContainer<TValue> _valueEventStreamContainer;
        public PointerValueEventStreamContainer<TValue> ValueEventStreamContainer => _valueEventStreamContainer;
        
        [SerializeField] private TrackingMode _trackingMode;

        [SerializeField] private bool _endOnExit;
        
        private readonly Dictionary<int, Guid> _pointerEvents = new();
        
        private readonly Dictionary<Guid, BehaviorSubject<PointerValueEvent<TValue>>> _valueEventSubjects = new();

        private void BeginEvent(PointerEventData eventData)
        {
            if (!Extract(true, eventData, out TValue value))
            {
                return;
            }
            
            Guid eventId = _driver.BeginStream(value);
            _pointerEvents[eventData.pointerId] = eventId; 

            BehaviorSubject<PointerValueEvent<TValue>> subject = new BehaviorSubject<PointerValueEvent<TValue>>(new PointerValueEvent<TValue>(eventId, value, eventData));
            _valueEventSubjects[eventId] = subject;

            if (EventStreamContainer)
            {
                EventStreamContainer.StreamNode.Push(eventId, subject.Select(valueEvent => new PointerEvent(valueEvent.Id, eventData))); 
            }
            if (ValueEventStreamContainer)
            {
                ValueEventStreamContainer.StreamNode.Push(eventId, subject.AsObservable()); 
            }
        }

        private void UpdateEvent(PointerEventData eventData)
        {
            if (!_pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId))
            {
                return;
            }

            if (!Extract(false, eventData, out TValue value))
            {
                return;
            }
            
            _driver.UpdateStream(eventId, value);
            
            if (!_valueEventSubjects.TryGetValue(eventId, out BehaviorSubject<PointerValueEvent<TValue>> subject))
            {
                return;
            }
            
            subject.OnNext(new PointerValueEvent<TValue>(eventId, value, eventData));
        }
        
        private void EndEvent(PointerEventData eventData)
        {
            End(eventData);
            
            if (!_pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId))
            {
                return;
            }
            
            _driver.EndStream(eventId);
            _pointerEvents.Remove(eventData.pointerId);

            if (!_valueEventSubjects.TryGetValue(eventId, out BehaviorSubject<PointerValueEvent<TValue>> subject))
            {
                return;
            }
            
            subject.OnCompleted();
            subject.Dispose();
            _valueEventSubjects.Remove(eventId);
        }

        protected abstract bool Extract(bool initial, PointerEventData eventData, out TValue value);

        protected virtual void End(PointerEventData eventData)
        {
            
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            EndEvent(eventData);
            BeginEvent(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            EndEvent(eventData);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (_trackingMode != TrackingMode.Move)
            {
                return;
            }
            
            UpdateEvent(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_endOnExit)
            {
                EndEvent(eventData);
            }
        }
        
        public void OnScroll(PointerEventData eventData)
        {
            UpdateEvent(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_trackingMode != TrackingMode.Drag)
            {
                return;
            }
            
            // Debug.Log($"{this} {nameof(OnDrag)} {eventData}");
            
            UpdateEvent(eventData);
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }
    }
}