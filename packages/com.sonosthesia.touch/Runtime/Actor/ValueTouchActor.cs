using Sonosthesia.Interaction;
using Sonosthesia.Utils;

namespace Sonosthesia.Touch
{
    public class ValueTouchActor<TValue> : TouchActor, IValueEventStreamContainer<TValue, TriggerValueEvent<TValue>> 
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