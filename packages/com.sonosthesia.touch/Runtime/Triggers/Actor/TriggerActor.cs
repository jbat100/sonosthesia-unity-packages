using Sonosthesia.Utils;

namespace Sonosthesia.Touch
{
    public class TriggerActor<TValue> : BaseTriggerActor where TValue : struct
    {
        private StreamNode<TriggerValueEvent<TValue>> _valueStreamNode;
        public StreamNode<TriggerValueEvent<TValue>> ValueStreamNode => _valueStreamNode ??= new StreamNode<TriggerValueEvent<TValue>>(this);
    }
}