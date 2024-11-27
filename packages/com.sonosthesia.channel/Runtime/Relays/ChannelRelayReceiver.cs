using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Channel
{
    public class ChannelRelayReceiver<T> : Channel<T> where T : struct
    {
        [SerializeField] private ChannelRelay<T> _relay;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _relay.StreamObservable.Subscribe(Push);
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}