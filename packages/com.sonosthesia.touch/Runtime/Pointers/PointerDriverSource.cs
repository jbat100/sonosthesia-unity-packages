using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sonosthesia.Channel;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    public abstract class PointerDriverSource<TValue> : MonoBehaviour, 
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
        public readonly struct SourceEvent
        {
            public readonly Guid Id;
            public readonly TValue Value;
            public readonly PointerEventData Data;

            public SourceEvent(Guid id, TValue value, PointerEventData data)
            {
                Id = id;
                Value = value;
                Data = data;
            }
        }

        private readonly Dictionary<Guid, BehaviorSubject<SourceEvent>> _eventSubjects = new();
        private readonly Subject<IObservable<SourceEvent>> _eventStreamSubject = new();

        private readonly ReactiveCollection<Guid> _ongoingEvents = new();
        public IReadOnlyReactiveCollection<Guid> OngoingEvents => _ongoingEvents; 

        public IObservable<IObservable<SourceEvent>> StreamObservable => _eventStreamSubject.AsObservable();

        private void BeginEvent(PointerEventData eventData)
        {
            if (!Extract(true, eventData, out TValue value))
            {
                return;
            }
            
            Guid eventId = _driver.BeginEvent(value);
            _pointerEvents[eventData.pointerId] = eventId; 

            BehaviorSubject<SourceEvent> subject = new BehaviorSubject<SourceEvent>(new SourceEvent(eventId, value, eventData));
            _eventSubjects[eventId] = subject;
            _eventStreamSubject.OnNext(subject);
            
            _ongoingEvents.Add(eventId);
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
            
            if (!_eventSubjects.TryGetValue(eventId, out BehaviorSubject<SourceEvent> subject))
            {
                return;
            }
            
            subject.OnNext(new SourceEvent(eventId, value, eventData));
        }
        
        private void EndEvent(PointerEventData eventData)
        {
            if (!_pointerEvents.TryGetValue(eventData.pointerId, out Guid eventId))
            {
                return;
            }
            
            _driver.EndEvent(eventId);
            _pointerEvents.Remove(eventData.pointerId);

            if (!_eventSubjects.TryGetValue(eventId, out BehaviorSubject<SourceEvent> subject))
            {
                return;
            }
            
            subject.OnCompleted();
            subject.Dispose();
            _eventSubjects.Remove(eventId);

            _ongoingEvents.Remove(eventId);
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