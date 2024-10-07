using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Sonosthesia.Channel;
using Sonosthesia.Interaction;

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
        
        private readonly Dictionary<int, Guid> _pointerStreams = new();
        
        private readonly Dictionary<Guid, BehaviorSubject<ValueEvent<TValue, PointerEvent>>> _valueEventSubjects = new();

        private void BeginEvent(PointerEventData eventData)
        {
            if (!Extract(true, eventData, out TValue value))
            {
                return;
            }
            
            Guid id = _driver.BeginStream(value);
            _pointerStreams[eventData.pointerId] = id;

            PointerEvent pointerEvent = new PointerEvent(eventData);
            BehaviorSubject<ValueEvent<TValue, PointerEvent>> subject = new (new ValueEvent<TValue, PointerEvent>(value, pointerEvent));
            _valueEventSubjects[id] = subject;

            if (EventStreamContainer)
            {
                EventStreamContainer.StreamNode.Push(id, subject.Select(valueEvent => valueEvent.Event)); 
            }
            if (ValueEventStreamContainer)
            {
                ValueEventStreamContainer.StreamNode.Push(id, subject.AsObservable()); 
            }
        }

        private void UpdateEvent(PointerEventData eventData)
        {
            if (!_pointerStreams.TryGetValue(eventData.pointerId, out Guid id))
            {
                return;
            }

            if (!Extract(false, eventData, out TValue value))
            {
                return;
            }
            
            _driver.UpdateStream(id, value);
            
            if (!_valueEventSubjects.TryGetValue(id, out BehaviorSubject<ValueEvent<TValue, PointerEvent>> subject))
            {
                return;
            }
            
            subject.OnNext(new ValueEvent<TValue, PointerEvent>(value, new PointerEvent(eventData)));
        }
        
        private void EndEvent(PointerEventData eventData)
        {
            End(eventData);
            
            if (!_pointerStreams.TryGetValue(eventData.pointerId, out Guid eventId))
            {
                return;
            }
            
            _driver.EndStream(eventId);
            _pointerStreams.Remove(eventData.pointerId);

            if (!_valueEventSubjects.TryGetValue(eventId, out BehaviorSubject<ValueEvent<TValue, PointerEvent>> subject))
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