using System;
using Sonosthesia.Interaction;

namespace Sonosthesia.Touch
{
    public readonly struct TriggerValueEvent<TValue> : IValueEvent<TValue> where TValue : struct
    {
        public readonly Guid Id;
        public readonly ITouchData TouchData;
        public readonly TValue Value;
        public readonly float StartTime;

        public TriggerValueEvent(TouchEvent touchEvent, TValue value)
        {
            Id = touchEvent.Id;
            StartTime = touchEvent.StartTime;
            TouchData = touchEvent.TouchData;
            Value = value;
        }
        
        public TriggerValueEvent(Guid id, ITouchData touchData, TValue value, float startTime)
        {
            Id = id;
            TouchData = touchData;
            Value = value;
            StartTime = startTime;
        }

        public TriggerValueEvent<TValue> Update(TValue value)
        {
            return new TriggerValueEvent<TValue>(Id, TouchData, value, StartTime);
        }
        
        public void EndStream()
        {
            TouchData.Source.KillStream(Id);
        }

        public TValue GetValue() => Value;
    }
    
    public class TouchValueEventStreamContainer<TValue> : ValueEventStreamContainer<TValue, TriggerValueEvent<TValue>>
        where TValue : struct
    {
        
    }
}