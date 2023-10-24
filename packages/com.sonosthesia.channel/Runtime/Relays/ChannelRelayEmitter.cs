using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Channel
{
    public class ChannelRelayEmitter<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private Channel<T> _source;

        [SerializeField] private ChannelRelay<T> _relay;

        private IDisposable _subscription;
        
        protected void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _source.StreamObservable.Subscribe(stream => _relay.Pipe(stream));
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}