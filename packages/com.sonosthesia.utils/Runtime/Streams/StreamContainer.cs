using UnityEngine;

namespace Sonosthesia.Utils
{
    public class StreamContainer<T> : MonoBehaviour where T : struct 
    {
        private StreamNode<T> _streamNode;
        public StreamNode<T> StreamNode => _streamNode ??= new StreamNode<T>(this);

        protected virtual void OnDestroy()
        {
            _streamNode?.Dispose();
        }
    }
}