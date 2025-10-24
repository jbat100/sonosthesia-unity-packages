using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public enum TouchStart
    {
        /// <summary>
        /// Touch was started on trigger enter
        /// </summary>
        Enter,
        
        /// <summary>
        /// Touch was started by gate switch on trigger stay
        /// </summary>
        Deferred,
        
        /// <summary>
        /// Touch was started on actor (or shortly after) OnEnable
        /// </summary>
        Trojan
    }
    
    public interface ITouchData
    {
        TouchStart Start { get; }
        Collider Collider { get; }
        bool Colliding { get; }
        ATouchSource Source { get; }
        TouchActor Actor { get; }
    }

    // used for affordances
    public readonly struct TouchEvent : IInteractionEvent
    {
        public readonly ITouchData touchData;
        public readonly float startTime;

        public float Age => Time.time - startTime;
        
        float IInteractionEvent.StartTime => startTime;
        IInteractionEndpoint IInteractionEvent.Source => touchData?.Source;
        IInteractionEndpoint IInteractionEvent.Actor => touchData?.Actor;

        public TouchEvent(ITouchData touchData, float startTime)
        {
            this.touchData = touchData;
            this.startTime = startTime;
        }

        public override string ToString()
        {
            return $"{nameof(TouchEvent)} ({touchData.Start}) {nameof(Age)} {Age}";
        }
    }
}