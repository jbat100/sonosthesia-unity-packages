using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class ValueEventStreamContainer<TValue, TEvent> : MonoBehaviour, IValueEventStreamContainer<TValue, TEvent> 
        where TValue : struct 
        where TEvent : struct, IValueEvent<TValue>
    {
        private StreamNode<TEvent> _valueStreamNode;
        public StreamNode<TEvent> StreamNode => _valueStreamNode ??= new StreamNode<TEvent>(this);

        protected virtual void OnDestroy()
        {
            _valueStreamNode?.Dispose();
        }
    }
}