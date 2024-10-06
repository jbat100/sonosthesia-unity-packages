using Sonosthesia.Interaction;

namespace Sonosthesia.Touch
{
    public readonly struct TriggerValueEvent<TValue> : IValueEvent<TValue> where TValue : struct
    {
        public readonly ITouchData TouchData;
        public readonly TValue Value;
        public readonly float StartTime;

        public TriggerValueEvent(TouchEvent touchEvent, TValue value)
        {
            StartTime = touchEvent.StartTime;
            TouchData = touchEvent.TouchData;
            Value = value;
        }
        
        public TriggerValueEvent(ITouchData touchData, TValue value, float startTime)
        {
            TouchData = touchData;
            Value = value;
            StartTime = startTime;
        }

        public TriggerValueEvent<TValue> Update(TValue value)
        {
            return new TriggerValueEvent<TValue>(TouchData, value, StartTime);
        }

        public TValue GetValue() => Value;
    }
    
    public class TouchValueEventStreamContainer<TValue> : ValueEventStreamContainer<TValue, TriggerValueEvent<TValue>>
        where TValue : struct
    {
        
    }
}