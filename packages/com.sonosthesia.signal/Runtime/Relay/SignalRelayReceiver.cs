using System;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sonosthesia.Signal
{
    public class SignalRelayReceiver<T> : Signal<T> where T : struct
    {
        [FormerlySerializedAs("signalRelay")] 
        [SerializeField] private SignalRelay<T> _signalRelay;

        private IDisposable _subscription;

        private void SetupRelay()
        {
            _subscription?.Dispose();
            if (_signalRelay)
            {
                _subscription = _signalRelay.Observable.Subscribe(Broadcast);    
            }
        }

        protected virtual void OnValidate() => SetupRelay();
        
        protected virtual void OnEnable() => SetupRelay();

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}