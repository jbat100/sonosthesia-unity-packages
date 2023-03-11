using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class RelayEmitter<T> : MonoBehaviour where T: struct
    {
        [SerializeField] private Signal<T> _source;

        [SerializeField] private Relay<T> _relay;

        private IDisposable _subscription;

        protected void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _source.SignalObservable.Subscribe(value => _relay.Broadcast(value));
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}