using System;
using UnityEngine;
using Sonosthesia.Signal;
using UniRx;

namespace Sonosthesia.Generator
{
    public class GeneratorSignal<T> : Signal<T> where T : struct
    {
        [SerializeField] private Generator<T> _generator;

        [SerializeField] private Signal<float> _timeSignal;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_timeSignal)
            {
                _timeSignal.SignalObservable.Subscribe(time =>
                {
                    T raw = _generator.Evaluate(time);
                    Broadcast(PostProcess(raw));
                });
            }
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        protected virtual T PostProcess(T value) => value;
    }
}