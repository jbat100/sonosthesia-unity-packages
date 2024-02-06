using System;
using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    // used for affordances
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
    
    /// <summary>
    /// Base class allows non templated affordances to access event streams
    /// </summary>
    public class BasePointerSource : MonoBehaviour, IEventStreamContainer<PointerEvent>
    {
        private StreamNode<PointerEvent> _eventStreamNode;
        public StreamNode<PointerEvent> EventStreamNode => _eventStreamNode ??= new StreamNode<PointerEvent>(this);

        protected virtual void OnDestroy()
        {
            _eventStreamNode?.Dispose();
        }
    }
}