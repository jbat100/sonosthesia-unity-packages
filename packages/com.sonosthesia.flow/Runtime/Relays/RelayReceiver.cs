using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class RelayReceiver<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private Signal<T> _target;

        [SerializeField] private Relay<T> _relay;

        private IDisposable _subscription;

        protected void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _relay.Observable.Subscribe(value => _target.Broadcast(value));
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}