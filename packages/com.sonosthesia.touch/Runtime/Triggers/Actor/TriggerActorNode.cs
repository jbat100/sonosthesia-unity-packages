using System;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    // nodes form trees where each nodes gets trigger events from all descendants allowing 
    // arbitrary groups to be formed
    
    public class TriggerActorNode<TValue> : BaseTriggerActor where TValue : struct
    {
        private StreamNode<TriggerValueEvent<TValue>> _valueStreamNode;
        public StreamNode<TriggerValueEvent<TValue>> ValueStreamNode => _valueStreamNode ??= new StreamNode<TriggerValueEvent<TValue>>(this);
        
        [SerializeField] private TriggerActorNode<TValue> _parent;

        protected virtual void Awake()
        {
            if (_parent)
            {
                _parent = GetComponentInParent<TriggerActorNode<TValue>>();    
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _valueStreamNode?.Dispose();
            _valueStreamNode = null;
        }
        
        public override bool IsAvailable(Collider actor)
        {
            if (!base.IsAvailable(actor))
            {
                return false;
            }
            
            return !_parent || _parent.IsAvailable(actor);
        }
    }
}