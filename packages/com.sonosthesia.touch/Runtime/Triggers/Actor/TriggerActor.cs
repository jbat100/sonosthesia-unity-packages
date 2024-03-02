using System;
using System.Collections.Generic;

namespace Sonosthesia.Touch
{
    public class TriggerActor<TValue> : ValueTriggerEndpoint<TValue> where TValue : struct
    {
        public override void EndAllStreams()
        {
            Dictionary<Guid, TriggerValueEvent<TValue>> values 
                = new Dictionary<Guid, TriggerValueEvent<TValue>>(ValueStreamNode.Values);

            foreach (KeyValuePair<Guid, TriggerValueEvent<TValue>> pair in values)
            {
                pair.Value.TriggerData.Source.EndStream(pair.Key);
            }
        }

        public override void EndStream(Guid id)
        {
            if (ValueStreamNode.Values.TryGetValue(id, out TriggerValueEvent<TValue> e))
            {
                e.TriggerData.Source.EndStream(id);
            }
        }
    }
}