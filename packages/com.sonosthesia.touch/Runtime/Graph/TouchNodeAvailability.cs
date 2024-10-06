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
    
    [RequireComponent(typeof(TouchNode))]
    public class TouchNodeAvailability : Signal<NodeAvailabilityState>
    {
        private TouchNode _node;

        private IDisposable _subscription;
        
        protected virtual void Awake()
        {
            _node = GetComponent<TouchNode>();
        }

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _node.StreamNode.StreamObservable.AsUnitObservable()
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
            TouchNode current = _node;
            bool maxReached = false;
            while (current)
            {
                if (current.StreamNode.Values.Count >= current.MaxConcurrent)
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