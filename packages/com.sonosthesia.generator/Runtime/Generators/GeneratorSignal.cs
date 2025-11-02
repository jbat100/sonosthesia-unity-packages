using System;
using Sonosthesia.Processing;
using UnityEngine;
using Sonosthesia.Signal;
using UniRx;

namespace Sonosthesia.Generator
{
    public class GeneratorSignal<TValue, TProcessor> : Signal<TValue> where TValue : struct where TProcessor : IProcessor<TValue>
    {
        [SerializeField] private Generator<TValue> _generator;

        [SerializeField] private Signal<float> _timeSignal;

        [SerializeField] private TProcessor _processor;
        
        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_timeSignal)
            {
                _timeSignal.SignalObservable.Subscribe(time =>
                {
                    TValue raw = _generator.Evaluate(time);
                    Broadcast(_processor.Process(raw));
                });
            }
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }

    public class GeneratorSignal<TValue> : GeneratorSignal<TValue, PassthroughProcessor<TValue>> where TValue : struct
    {
        
    }
}