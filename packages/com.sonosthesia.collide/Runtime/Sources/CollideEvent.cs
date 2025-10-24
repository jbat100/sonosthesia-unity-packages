using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Collide
{
    public readonly struct CollideEvent : IInteractionEvent
    {
        public readonly Collision Collision;
        public readonly float StartTime;
        public readonly CollideSource Source;
        public readonly CollideActor Actor;
        
        float IInteractionEvent.StartTime => StartTime;
        IInteractionEndpoint IInteractionEvent.Source => Source;
        IInteractionEndpoint IInteractionEvent.Actor => Actor;

        public CollideEvent(Collision collision, float startTime, CollideSource source, CollideActor actor)
        {
            Collision = collision;
            StartTime = startTime;
            Source = source;
            Actor = actor;
        }
    }
}