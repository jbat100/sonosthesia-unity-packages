using System;
using Sonosthesia.Utils;
using Sonosthesia.Signal;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    public class SignalTrigger<T, TExtractor> : MonoBehaviour, ILogSwitch where T : struct where TExtractor : IExtractor<T>
    {
        [SerializeField] private bool _log;
        public bool Log => _log;
        
        [SerializeField] private Signal<T> _source;
        
        [SerializeField] private Trigger _destination;
        
        [SerializeField] private SignalTriggerConfiguration<T, TExtractor> _configuration;
        
        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _source.SignalObservable.Subscribe(source =>
            {
                this.LogVerbose($"{this} trigger on {source}");
                _configuration.Trigger(_destination.TriggerController, source);
            });
        }

        protected virtual void OnDisable() => _subscription?.Dispose();
    }
}