using UnityEngine;

namespace Sonosthesia.Collide
{
    public readonly struct CollideEvent
    {
        public readonly Collision Collision;
        public readonly float StartTime;
        public readonly CollideSource Source;
        public readonly CollideActor Actor;

        public CollideEvent(Collision collision, float startTime, CollideSource source, CollideActor actor)
        {
            Collision = collision;
            StartTime = startTime;
            Source = source;
            Actor = actor;
        }
    }
}