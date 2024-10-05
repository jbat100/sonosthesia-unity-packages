using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TriggerStream : MonoBehaviour, IEventStreamContainer<TriggerEvent>
    {
        private StreamNode<TriggerEvent> _eventStreamNode;
        public StreamNode<TriggerEvent> EventStreamNode => _eventStreamNode ??= new StreamNode<TriggerEvent>(this);
        
        protected virtual void OnDestroy() => _eventStreamNode?.Dispose();
    }
}