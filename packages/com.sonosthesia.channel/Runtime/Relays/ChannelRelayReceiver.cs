using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class ChannelRelayReceiver<T> : Channel<T> where T : struct
    {
        [SerializeField] private ChannelRelay<T> _relay;

        private IDisposable _subscription;

        protected override void OnEnable()
        {
            base.OnEnable();
            _subscription?.Dispose();
            _subscription = _relay.StreamObservable.Subscribe(Pipe);
        }

        protected override void OnDisable()
        {
            base.OnEnable();
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}