using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Touch
{
    public enum NodeAvailabilityState
    {
        Invalid,
            
        // permission request will fail
        Unavailable,
        
        // request will succeed 
        Available,
        
        // request will succeed but will another stream
        Switchable
    }
    
    [RequireComponent(typeof(TriggerNode))]
    public class TriggerNodeAvailability : Signal<NodeAvailabilityState>
    {
        private TriggerNode _node;

        private IDisposable _subscription;
        
        protected virtual void Awake()
        {
            _node = GetComponent<TriggerNode>();
        }

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _node.EventStreamNode.StreamObservable.AsUnitObservable()
                .Merge(_node.UpstreamObservable)
                .StartWith(Unit.Default)
                .BatchFrame()
                .Subscribe(_ => Broadcast(ComputeAvalability()));
        }
        
        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        protected virtual NodeAvailabilityState ComputeAvalability()
        {
            TriggerNode current = _node;
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
                return NodeAvailabilityState.Available;
            }

            return current.AllowSwitching ? NodeAvailabilityState.Switchable : NodeAvailabilityState.Unavailable;
        }
    }
}