using System;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sonosthesia.Signal
{
    public class SignalRelayEmitter<T> : MonoBehaviour where T: struct
    {
        [SerializeField] private Signal<T> _source;

        [FormerlySerializedAs("signalRelay")] 
        [SerializeField] private SignalRelay<T> _signalRelay;

        private IDisposable _subscription;

        protected virtual void OnValidate() => SetupRelay();
        
        protected virtual void OnEnable() => SetupRelay();

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
        
        private void SetupRelay()
        {
            _subscription?.Dispose();
            if (_source && _signalRelay)
            {
                _subscription = _source.SignalObservable.Subscribe(value => _signalRelay.Broadcast(value));   
            }
        }

    }
}