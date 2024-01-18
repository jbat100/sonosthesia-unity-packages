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
        BaseTriggerSource Source { get; }
        BaseTriggerActor Actor { get; }
    }
    
    // used for affordances
    public readonly struct TriggerSourceEvent
    {
        public readonly Guid Id;
        public readonly ITriggerData TriggerData;
        public readonly float StartTime;

        public TriggerSourceEvent(Guid id, ITriggerData triggerData, float startTime)
        {
            Id = id;
            TriggerData = triggerData;
            StartTime = startTime;
        }

        public void EndStream()
        {
            TriggerData.Source.EndStream(Id);
        }
    }
    
    public abstract class BaseTriggerSource : MonoBehaviour, IStreamSource<TriggerSourceEvent>
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