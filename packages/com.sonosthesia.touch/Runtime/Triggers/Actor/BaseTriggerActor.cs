using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class BaseTriggerActor : MonoBehaviour
    {
        [SerializeField] private int _maxConcurrent = 1;

        // can be used to filter actors or to allow one source to have different responses 
        [SerializeField] private int _domain;
        
        private StreamNode<TriggerSourceEvent> _sourceStreamNode;
        public StreamNode<TriggerSourceEvent> SourceStreamNode => _sourceStreamNode ??= new StreamNode<TriggerSourceEvent>(this);
        
        public virtual bool IsAvailable(Collider other)
        {
            if (!isActiveAndEnabled)
            {
                return false;
            }
            
            return SourceStreamNode.Values.Count < _maxConcurrent;
        }
        
        protected virtual void OnDestroy()
        {
            _sourceStreamNode?.Dispose();
            _sourceStreamNode = null;
        }
    }
}