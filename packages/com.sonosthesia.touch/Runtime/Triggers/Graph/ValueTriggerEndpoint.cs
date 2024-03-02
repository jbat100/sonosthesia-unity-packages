using System;
using Sonosthesia.Utils;

namespace Sonosthesia.Touch
{
    public readonly struct TriggerValueEvent<TValue> : IValueEvent<TValue> where TValue : struct
    {
        public readonly Guid Id;
        public readonly ITriggerData TriggerData;
        public readonly TValue Value;
        public readonly float StartTime;

        public TriggerValueEvent(Guid id, ITriggerData triggerData, TValue value, float startTime)
        {
            Id = id;
            TriggerData = triggerData;
            Value = value;
            StartTime = startTime;
        }

        public TriggerValueEvent<TValue> Update(TValue value)
        {
            return new TriggerValueEvent<TValue>(Id, TriggerData, value, StartTime);
        }
        
        public void EndStream()
        {
            TriggerData.Source.EndStream(Id);
        }

        public TValue GetValue() => Value;
    }
    
    public abstract class ValueTriggerEndpoint<TValue> : TriggerEndpoint,
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