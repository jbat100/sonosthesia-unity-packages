using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [RequireComponent(typeof(BaseTriggerGraphNode))]
    public class BaseTriggerGraphNodeAvailability : MonoBehaviour
    {
        public enum AvailabilityState
        {
            Invalid,
            
            // permission request will fail
            Unavailable,
        
            // request will succeed 
            Available,
        
            // request will succeed but will another stream
            Switchable
        }
        
        private readonly ReactiveProperty<AvailabilityState> _availability = new (AvailabilityState.Invalid);
        public IReadOnlyReactiveProperty<AvailabilityState> Availability { get; private set; }
        
        private BaseTriggerGraphNode _node;

        private IDisposable _subscription;
        
        protected virtual void Awake()
        {
            Availability = _availability.ToReactiveProperty();
            _node = GetComponent<BaseTriggerGraphNode>();
        }

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _node.SourceStreamNode.StreamObservable.AsUnitObservable()
                .Merge(_node.UpstreamObservable)
                .StartWith(Unit.Default)
                .BatchFrame()
                .Subscribe(_ => _availability.Value = ComputeAvalability());
        }
        
        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        protected virtual AvailabilityState ComputeAvalability()
        {
            BaseTriggerGraphNode current = _node;
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
                return AvailabilityState.Available;
            }

            return current.AllowSwitching ? AvailabilityState.Switchable : AvailabilityState.Unavailable;
        }
    }
}