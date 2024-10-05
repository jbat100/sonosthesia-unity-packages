using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class ValueTouchActor<TValue> : TouchActor, IValueEventStreamContainer<TValue, TriggerValueEvent<TValue>> 
        where TValue : struct
    {
        // we use composition with ValueTouchEndpoint so that affordances can apply to both sources and actors
        
        [SerializeField] private ValueTouchEndpoint<TValue> _endpoint;
        
        public StreamNode<TriggerValueEvent<TValue>> ValueStreamNode => _endpoint ? _endpoint.ValueStreamNode : null;
    }
}