using System;
using Sonosthesia.Utils;
using Sonosthesia.Signal;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    public class SignalTrigger<T> : MonoBehaviour, ILogSwitch where T : struct
    {
        [SerializeField] private bool _log;
        public bool Log => _log;

        [SerializeField] private InterfaceReference<ISignal<T>> _source;
        
        [SerializeField] private Trigger _destination;
        
        [SerializeField] private InterfaceReference<ISignalTriggerConfiguration<T>> _configuration;
        
        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _source.Value.Observable.Subscribe(source =>
            {
                this.LogVerbose($"{this} trigger on {source}");
                _configuration.Value.Trigger(_destination.TriggerImplementation, source);
            });
        }

        protected virtual void OnDisable() => _subscription?.Dispose();
    }
}