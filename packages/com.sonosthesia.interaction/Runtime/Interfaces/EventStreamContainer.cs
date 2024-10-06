using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class EventStreamContainer<TEvent> : MonoBehaviour, IEventStreamContainer<TEvent> where TEvent : struct
    {
        private StreamNode<TEvent> _StreamNode;
        public StreamNode<TEvent> StreamNode => _StreamNode ??= new StreamNode<TEvent>(this);

        protected void OnDestroy()
        {
            _StreamNode?.Dispose();
        }
    }
}