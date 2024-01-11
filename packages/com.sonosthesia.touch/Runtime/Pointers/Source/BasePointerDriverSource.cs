using System;
using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    // used for affordances
    public readonly struct PointerSourceEvent
    {
        public readonly Guid Id;
        public readonly PointerEventData Data;

        public PointerSourceEvent(Guid id, PointerEventData data)
        {
            Id = id;
            Data = data;
        }
    }
    
    /// <summary>
    /// Base class allows non templated affordances to access event streams
    /// </summary>
    public class BasePointerDriverSource : MonoBehaviour
    {
        private StreamNode<PointerSourceEvent> _sourceStreamNode;
        public StreamNode<PointerSourceEvent> SourceStreamNode => _sourceStreamNode ??= new StreamNode<PointerSourceEvent>(this);

        protected virtual void OnDestroy()
        {
            _sourceStreamNode?.Dispose();
        }
    }
}