using System;
using System.Collections.Generic;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Pointer
{
    public class PointerSource : InteractionEndpoint, 
        IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, 
        IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Channel.Channel<PointerEvent> _eventChannel;
        public Channel.Channel<PointerEvent> EventChannel => _eventChannel;
        
        private readonly Dictionary<int, InternalData> _internal = new();

        private class InternalData
        {
            public BehaviorSubject<PointerEvent> EventSubject;
            public float StartTime;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            PointerEvent pointerEvent = new PointerEvent(eventData, this, Time.time);

            InternalData internalData = new InternalData()
            {
                StartTime = Time.time,
                EventSubject = new BehaviorSubject<PointerEvent>(pointerEvent)
            };

            _internal[eventData.pointerId] = internalData;

            this.LogWarning($"{this} new stream on {nameof(OnPointerDown)} {eventData}");
            
            _eventChannel.Push(Guid.NewGuid(), internalData.EventSubject);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_internal.TryGetValue(eventData.pointerId, out InternalData internalData))
            {
                return;
            }
            
            internalData.EventSubject.OnNext(new PointerEvent(eventData, this, internalData.StartTime));
            internalData.EventSubject.OnCompleted();
            internalData.EventSubject.Dispose();
            
            this.LogWarning($"{this} end stream on {nameof(OnPointerUp)} {eventData}");
            
            _internal.Remove(eventData.pointerId);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (!_internal.TryGetValue(eventData.pointerId, out InternalData internalData))
            {
                return;
            }
            
            this.LogVerbose($"{this} update stream on {nameof(OnPointerMove)} {eventData}");
            
            internalData.EventSubject.OnNext(new PointerEvent(eventData, this, internalData.StartTime));
        }

        public void OnDrag(PointerEventData eventData)
        {
            this.LogVerbose($"{this} {nameof(OnDrag)} {eventData}");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            this.LogVerbose($"{this} {nameof(OnDrag)} {eventData}");
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            this.LogVerbose($"{this} {nameof(OnDrag)} {eventData}");
        }
    }
}