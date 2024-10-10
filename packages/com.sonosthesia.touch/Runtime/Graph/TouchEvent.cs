using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public interface ITouchData
    {
        Collider Collider { get; }
        bool Colliding { get; }
        TouchSource Source { get; }
        TouchActor Actor { get; }
    }
    
    // used for affordances
    public readonly struct TouchEvent : IInteractionEvent
    {
        public readonly ITouchData TouchData;
        public readonly float StartTime;
        
        public IInteractionEndpoint Source => TouchData?.Source;
        public IInteractionEndpoint Actor => TouchData?.Actor;

        public TouchEvent(ITouchData touchData, float startTime)
        {
            TouchData = touchData;
            StartTime = startTime;
        }
    }
    
    public static class TouchEventExtensions
    {
        public static Vector3 ActorPositionInSourceSpace(this TouchEvent touchEvent)
        {
            return touchEvent.TouchData.Source.transform
                .InverseTransformPoint(touchEvent.TouchData.Actor.transform.position);
        }
    }
}