using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchEventStreamContainer : MonoBehaviour, IEventStreamContainer<TouchEvent>
    {
        private StreamNode<TouchEvent> _streamNode;
        public StreamNode<TouchEvent> StreamNode => _streamNode ??= new StreamNode<TouchEvent>(this);
        
        protected virtual void OnDestroy() => _streamNode?.Dispose();
    }
}