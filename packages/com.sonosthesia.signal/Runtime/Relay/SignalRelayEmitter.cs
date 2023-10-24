using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class SignalRelayEmitter<T> : MonoBehaviour where T: struct
    {
        [SerializeField] private Signal<T> _source;

        [SerializeField] private SignalRelay<T> signalRelay;

        private IDisposable _subscription;

        protected void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _source.SignalObservable.Subscribe(value => signalRelay.Broadcast(value));
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}