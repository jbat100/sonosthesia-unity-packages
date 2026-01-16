using System;
using UnityEngine;
using UniRx;
using Sonosthesia.Signal;

namespace Sonosthesia.Channel
{
    public class ChannelCountSignal : StatelessSignal<float>
    {
        [SerializeField] private AbstractChannel _collector; 
    
        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _collector.Ids.ObserveCountChanged(true)
                .Subscribe(count => Broadcast((float)count));
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}