using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchNode : TouchEventStreamContainer
    {
        [Tooltip("Maximum concurrent streams for this node and descendents")]
        [SerializeField] private int _maxConcurrent = 1;
        public int MaxConcurrent => _maxConcurrent;
        
        [Tooltip("Allow ending one stream to create another while on max concurrent")]
        [SerializeField] private bool _allowSwitching;
        public bool AllowSwitching => _allowSwitching;
        
        [Tooltip("Calculate concurrent count per collider")]
        [SerializeField] private bool _perCollider;
        public bool PerCollider => _perCollider;
        
        [SerializeField] private TouchNode _parent;
        public TouchNode Parent => _parent;
        
        private readonly HashSet<TouchNode> _children = new();

        private CompositeDisposable _parentSubscriptions = new();
        
        private readonly Subject<Unit> _upstreamSubject = new();
        public IObservable<Unit> UpstreamObservable => _upstreamSubject.AsObservable();

        protected virtual void OnEnable()
        {
            _parentSubscriptions.Clear();
            if (_parent)
            {
                _parent.RegisterChildNode(this);
                
                // used to relay streams up the node hierarchy
                _parentSubscriptions.Add(_parent.StreamNode.Pipe(StreamNode));
                
                // used to send events when something changed updstream
                _parentSubscriptions.Add(_parent
                    .StreamNode.Values.ObserveCountChanged().AsUnitObservable()
                    .Merge(_parent.UpstreamObservable)
                    .Subscribe(_upstreamSubject));
            }
        }

        protected virtual void OnDisable()
        {
            _parentSubscriptions.Clear();
            if (_parent)
            {
                _parent.UnregisterChildNode(this);   
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _upstreamSubject.OnCompleted();
            _upstreamSubject.Dispose();
        }

        private void RegisterChildNode(TouchNode node)
        {
            _children.Add(node);
        }
        
        private void UnregisterChildNode(TouchNode node)
        {
            _children.Remove(node);
        }

        public void EndOldestStream()
        {
            float lowestStartTime = float.MaxValue;
            Guid oldest = Guid.Empty;
            foreach (KeyValuePair<Guid, TouchEvent> pair in StreamNode.Values)
            {
                float startTime = pair.Value.StartTime;
                if (startTime < lowestStartTime)
                {
                    oldest = pair.Key;
                    lowestStartTime = startTime;
                }
            }

            if (oldest != Guid.Empty)
            {
                KillStream(oldest);
            }
        }

        public virtual bool RequestPermission(Collider other)
        {
            if (!isActiveAndEnabled)
            {
                return false;
            }
            
            TouchNode current = this;
            bool maxReached = false;
            while (current)
            {
                int count = _perCollider
                    ? current.StreamNode.Values.Count(n => n.Value.TouchData.Collider == other)
                    : current.StreamNode.Values.Count;
                
                if (count >= current.MaxConcurrent)
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
                current.EndOldestStream();
                return true;
            }

            return false;
        }
    }
}