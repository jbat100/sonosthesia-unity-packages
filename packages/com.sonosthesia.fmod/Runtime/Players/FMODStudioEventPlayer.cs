using System;
using FMODUnity;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public class FMODStudioEventPlayer<T> : MonoBehaviour, ILogSwitch where T : struct
    {
        [SerializeField] private bool _log;
        public bool Log => _log;
        
        [SerializeField] private StudioEventEmitter _emitter;

        [SerializeField] private Signal<T> _source;

        private IDisposable _subscription;
        
        protected void OnEnable()
        {
            _subscription?.Dispose();
            if (_source)
            {
                _subscription = _source.SignalObservable.Subscribe(value =>
                {
                    this.LogVerbose($"{this} playing sound on {value}");
                    _emitter.Play();
                });
            }
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}