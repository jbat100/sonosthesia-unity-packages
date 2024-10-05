using System;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;

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
            TouchData.Source.RequestKillStream(Id);
        }

        public TValue GetValue() => Value;
    }
    
    public abstract class ValueTouchEndpoint<TValue> : TouchEndpoint,
        IValueEventStreamContainer<TValue, TriggerValueEvent<TValue>> 
        where TValue : struct
    {
        private StreamNode<TriggerValueEvent<TValue>> _valueStreamNode;
        public StreamNode<TriggerValueEvent<TValue>> ValueStreamNode => _valueStreamNode ??= new StreamNode<TriggerValueEvent<TValue>>(this);
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _valueStreamNode?.Dispose();
        }
    }
}