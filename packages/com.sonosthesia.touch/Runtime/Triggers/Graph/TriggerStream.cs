using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TriggerStream : MonoBehaviour, IStreamSource<TriggerSourceEvent>
    {
        private StreamNode<TriggerSourceEvent> _sourceStreamNode;
        public StreamNode<TriggerSourceEvent> SourceStreamNode => _sourceStreamNode ??= new StreamNode<TriggerSourceEvent>(this);
        
        protected virtual void OnDestroy() => _sourceStreamNode?.Dispose();
    }
}