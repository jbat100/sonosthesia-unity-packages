using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class SignalRelayReceiver<T> : Signal<T> where T : struct
    {
        [SerializeField] private SignalRelay<T> signalRelay;

        private IDisposable _subscription;

        protected void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = signalRelay.Observable.Subscribe(Broadcast);
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}