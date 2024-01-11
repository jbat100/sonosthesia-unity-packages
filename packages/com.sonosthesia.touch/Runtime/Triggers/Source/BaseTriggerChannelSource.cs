using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    /// <summary>
    /// ITriggerData plays the same role as PointerEventData
    /// </summary>

    public interface ITriggerData
    {
        Collider Collider { get; }
        bool Colliding { get; }
        BaseTriggerChannelSource Source { get; }
        BaseTriggerActor Actor { get; }
    }
    
    // used for affordances
    public readonly struct TriggerSourceEvent
    {
        public readonly Guid Id;
        public readonly ITriggerData TriggerData;

        public TriggerSourceEvent(Guid id, ITriggerData triggerData)
        {
            Id = id;
            TriggerData = triggerData;
        }
    }
    
    public abstract class BaseTriggerChannelSource : MonoBehaviour
    {
        private StreamNode<TriggerSourceEvent> _sourceStreamNode;
        public StreamNode<TriggerSourceEvent> SourceStreamNode => _sourceStreamNode ??= new StreamNode<TriggerSourceEvent>(this);

        public abstract void EndAllStreams();

        public abstract void EndStream(Guid id);
        
        protected virtual void OnDestroy()
        {
            _sourceStreamNode?.Dispose();
        }
    }
}