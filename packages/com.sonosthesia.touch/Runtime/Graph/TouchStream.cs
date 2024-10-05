using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchStream : MonoBehaviour, IEventStreamContainer<TouchEvent>
    {
        private StreamNode<TouchEvent> _eventStreamNode;
        public StreamNode<TouchEvent> EventStreamNode => _eventStreamNode ??= new StreamNode<TouchEvent>(this);
        
        protected virtual void OnDestroy() => _eventStreamNode?.Dispose();
    }
}