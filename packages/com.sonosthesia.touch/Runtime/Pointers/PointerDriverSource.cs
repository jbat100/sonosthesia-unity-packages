using System;
using System.Collections.Generic;
using Sonosthesia.Channel;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    public abstract class PointerDriverSource<TValue> : BasePointerDriverSource, 
        IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerExitHandler,
        IDragHandler, IInitializePotentialDragHandler
        where TValue : struct
    {
        private enum TrackingMode
        {
            Move,
            Drag
        }
        
        [SerializeField] private ChannelDriver<TValue> _driver;

        [SerializeField] private TrackingMode _trackingMode;
        
        [SerializeField] private bool _endOnExit;
        
        private readonly Dictionary<int, Guid> _pointerEvents = new();

        // used for affordances
        public readonly struct ValueEvent
        {
            public readonly Guid Id;
            public readonly TValue Value;
            public readonly PointerEventData Data;

            public ValueEvent(Guid id, TValue value, PointerEventData data)
            {
                Id = id;
                Value = value;
                Data = data;
            }
        }

        private readonly Dictionary<Guid, BehaviorSubject<ValueEvent>> _valueEventSubjects = new();
        private readonly Subject<IObservable<ValueEvent>> _valueStreamSubject = new();

        public IObservable<IObservable<ValueEvent>> ValueStreamObservable => _valueStreamSubject.AsObservable();

        private void BeginEvent(PointerEventData eventData)
        {
            if (!Extract(true, eventData, out TValue value))
            {
                return;
            }
            
            Guid eventId = _driver.BeginEvent(value);
            _pointerEvents[eventData.pointerId] = eventId; 

            BehaviorSubject<ValueEvent> subject = new BehaviorSubject<ValueEvent>(new ValueEvent(eventId, value, eventData));
            _valueEventSubjects[eventId] = subject;
            _valueStreamSubject.OnNext(subject);
            
            Pipe(subject.Select(valueEvent => new SourceEvent(valueEvent.Id, eventData)));
            
            RegisterEvent(eventId);
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
            
            _driver.UpdateEvent(eventId, value);
            
            if (!_valueEventSubjects.TryGetValue(eventId, out BehaviorSubject<ValueEvent> subject))
            {
                return;
            }
            
            subject.OnNext(new ValueEvent(eventId, value, eventData));
        }
        
        private void EndEvent(PointerEventData eventData)
        {
            if (!_pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId))
            {
                return;
            }
            
            _driver.EndEvent(eventId);
            _pointerEvents.Remove(eventData.pointerId);

            if (!_valueEventSubjects.TryGetValue(eventId, out BehaviorSubject<ValueEvent> subject))
            {
                return;
            }
            
            subject.OnCompleted();
            subject.Dispose();
            _valueEventSubjects.Remove(eventId);

            UnregisterEvent(eventId);
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

        protected abstract bool Extract(bool initial, PointerEventData eventData, out TValue value);
        
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