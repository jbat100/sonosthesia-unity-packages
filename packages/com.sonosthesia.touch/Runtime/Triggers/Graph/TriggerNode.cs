using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TriggerNode : TriggerStream
    {
        [Tooltip("Maximum concurrent streams for this node and descendents")]
        [SerializeField] private int _maxConcurrent;
        public int MaxConcurrent => _maxConcurrent;
        
        [Tooltip("Allow ending one stream to create another while on max concurrent")]
        [SerializeField] private bool _allowSwitching;
        public bool AllowSwitching => _allowSwitching;
        
        [SerializeField] private TriggerNode _parent;
        public TriggerNode Parent => _parent;
        
        private readonly HashSet<TriggerNode> _children = new();

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
                _parentSubscriptions.Add(_parent.EventStreamNode.Pipe(EventStreamNode));
                
                // used to send events when something changed updstream
                _parentSubscriptions.Add(_parent
                    .EventStreamNode.Values.ObserveCountChanged().AsUnitObservable()
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

        private void RegisterChildNode(TriggerNode node)
        {
            _children.Add(node);
        }
        
        private void UnregisterChildNode(TriggerNode node)
        {
            _children.Remove(node);
        }

        public void EndOldestStream()
        {
            float lowestStartTime = float.MaxValue;
            TriggerEvent? oldest = null;
            foreach (KeyValuePair<Guid, TriggerEvent> pair in EventStreamNode.Values)
            {
                float startTime = pair.Value.StartTime;
                if (startTime < lowestStartTime)
                {
                    oldest = pair.Value;
                    lowestStartTime = startTime;
                }
            }

            if (oldest.HasValue)
            {
                oldest.Value.EndStream();
            }
        }
        
        public virtual bool RequestPermission(Collider other)
        {
            if (!isActiveAndEnabled)
            {
                return false;
            }
            
            TriggerNode current = this;
            bool maxReached = false;
            while (current)
            {
                if (current.EventStreamNode.Values.Count >= current.MaxConcurrent)
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