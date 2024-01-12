using System;
using System.Collections.Generic;
using Sonosthesia.Channel;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    
    // used for affordances
    public readonly struct PointerValueEvent<TValue> where TValue : struct
    {
        public readonly Guid Id;
        public readonly TValue Value;
        public readonly PointerEventData Data;

        public PointerValueEvent(Guid id, TValue value, PointerEventData data)
        {
            Id = id;
            Value = value;
            Data = data;
        }
    }
    
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
        

        private readonly Dictionary<Guid, BehaviorSubject<PointerValueEvent<TValue>>> _valueEventSubjects = new();
        
        private StreamNode<PointerValueEvent<TValue>> _valueStreamNode;
        public StreamNode<PointerValueEvent<TValue>> ValueStreamNode => _valueStreamNode ??= new StreamNode<PointerValueEvent<TValue>>(this);

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _valueStreamNode?.Dispose();
        }

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

            ValueStreamNode.Push(eventId, subject.AsObservable());
            SourceStreamNode.Push(eventId, subject.Select(valueEvent => new PointerSourceEvent(valueEvent.Id, eventData)));
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