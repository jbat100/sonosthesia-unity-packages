using System;
using Sonosthesia.Interaction;
using UnityEngine.EventSystems;

namespace Sonosthesia.Pointer
{
    public readonly struct PointerEvent
    {
        public readonly Guid Id;
        public readonly PointerEventData Data;

        public PointerEvent(Guid id, PointerEventData data)
        {
            Id = id;
            Data = data;
        }
    }
    
    public class PointerEventStreamContainer : EventStreamContainer<PointerEvent>
    {
        
    }
}