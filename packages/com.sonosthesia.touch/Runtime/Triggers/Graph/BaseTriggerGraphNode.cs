using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class BaseTriggerGraphNode : BaseTriggerStream
    {
        [Tooltip("Maximum concurrent streams for this node and descendents")]
        [SerializeField] private int _maxConcurrent;
        public int MaxConcurrent => _maxConcurrent;
        
        [Tooltip("Allow ending one stream to create another while on max concurrent")]
        [SerializeField] private bool _allowSwitching;
        public bool AllowSwitching => _allowSwitching;
        
        [SerializeField] private BaseTriggerGraphNode _parent;
        public BaseTriggerGraphNode Parent => _parent;
        
        private readonly HashSet<BaseTriggerGraphNode> _children = new();

        // notification for changes upstream
        
        private IDisposable _upstreamSubscription;
        private Subject<Unit> _upstreamSubject;
        public IObservable<Unit> UpstreamObservable => _upstreamSubject.AsObservable();

        protected virtual void OnEnable()
        {
            if (_parent)
            {
                _parent.RegisterChildNode(this);
                _upstreamSubscription = _parent
                    .SourceStreamNode.Values.ObserveCountChanged().AsUnitObservable()
                    .Merge(_parent.UpstreamObservable)
                    .Subscribe();
            }
        }

        protected virtual void OnDisable()
        {
            if (_parent)
            {
                _parent.UnregisterChildNode(this);   
                _upstreamSubscription?.Dispose();
            }
        }
        
        private void RegisterChildNode(BaseTriggerGraphNode node)
        {
            _children.Add(node);
        }
        
        private void UnregisterChildNode(BaseTriggerGraphNode node)
        {
            _children.Remove(node);
        }
        
        public virtual bool RequestPermission(Collider other)
        {
            if (!isActiveAndEnabled)
            {
                return false;
            }
            
       
            BaseTriggerGraphNode current = this;
            bool maxReached = false;
            while (current)
            {
                if (current.SourceStreamNode.Values.Count >= current.MaxConcurrent)
                {
                    maxReached = true;
                    break;
                }
                current = current.Parent;
            }

            if (!maxReached)
            {
                return true;
            }

            if (current.AllowSwitching)
            {
                
            }

            return false;
        }
    }
}