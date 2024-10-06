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
    public readonly struct TouchEvent
    {
        public readonly ITouchData TouchData;
        public readonly float StartTime;

        public TouchEvent(ITouchData touchData, float startTime)
        {
            TouchData = touchData;
            StartTime = startTime;
        }
    }
}