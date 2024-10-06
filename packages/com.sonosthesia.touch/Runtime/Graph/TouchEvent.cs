using System;
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
        public readonly Guid Id;
        public readonly ITouchData TouchData;
        public readonly float StartTime;

        public TouchEvent(Guid id, ITouchData touchData, float startTime)
        {
            Id = id;
            TouchData = touchData;
            StartTime = startTime;
        }

        public void EndStream()
        {
            TouchData.Source.KillStream(Id);
        }
    }
}