using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class RelayReceiver<T> : Signal<T> where T : struct
    {
        [SerializeField] private Relay<T> _relay;

        private IDisposable _subscription;

        protected void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _relay.Observable.Subscribe(Broadcast);
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}