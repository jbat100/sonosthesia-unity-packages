using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public enum TouchStart
    {
        Collision,
        Deferred
    }
    
    public interface ITouchData
    {
        TouchStart Start { get; }
        Collider Collider { get; }
        bool Colliding { get; }
        TouchSource Source { get; }
        TouchActor Actor { get; }
    }

    // used for affordances
    public readonly struct TouchEvent : IInteractionEvent
    {
        public readonly ITouchData touchData;
        public readonly float startTime;
        
        public float StartTime => startTime;
        public IInteractionEndpoint Source => touchData?.Source;
        public IInteractionEndpoint Actor => touchData?.Actor;

        public TouchEvent(ITouchData touchData, float startTime)
        {
            this.touchData = touchData;
            this.startTime = startTime;
        }
    }
    
    public static class TouchEventExtensions
    {
        public static Vector3 ActorPositionInSourceSpace(this TouchEvent touchEvent)
        {
            return touchEvent.touchData.Source.transform
                .InverseTransformPoint(touchEvent.touchData.Actor.transform.position);
        }
    }
}