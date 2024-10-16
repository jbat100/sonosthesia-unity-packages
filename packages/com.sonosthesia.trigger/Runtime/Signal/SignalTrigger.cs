using System;
using Sonosthesia.Envelope;
using UnityEngine;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UniRx;

namespace Sonosthesia.Trigger
{
    public class SignalTrigger<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private Signal<T> _source;
        
        [SerializeField] private Trigger _destination;

        [SerializeField] private Selector<T> _timeScaleSelector;

        [SerializeField] private Selector<T> _valueScaleSelector;

        [SerializeField] private EnvelopeFactory _envelope;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _source.SignalObservable.Subscribe(source =>
            {
                float timeScale = _timeScaleSelector ? _timeScaleSelector.Select(source) : 1f;
                float valueScale = _valueScaleSelector ? _valueScaleSelector.Select(source) : 1f;
                _destination.TriggerController.PlayTrigger(_envelope ? _envelope.Build() : null, valueScale, timeScale);
            });
        }

        protected virtual void OnDisable() => _subscription?.Dispose();
    }
}