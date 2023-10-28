using System;
using UnityEngine;
using UniRx;
using Sonosthesia.Signal;

namespace Sonosthesia.Channel
{
    public class ChannelCountSignal : Signal<float>
    {
        [SerializeField] private ChannelBase _channel; 
    
        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _channel.StreamIds.ObserveCountChanged(true)
                .Subscribe(count =>Broadcast((float)count));
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}