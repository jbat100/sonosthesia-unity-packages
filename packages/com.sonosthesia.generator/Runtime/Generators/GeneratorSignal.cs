using System;
using Sonosthesia.Processing;
using UnityEngine;
using Sonosthesia.Signal;
using UniRx;

namespace Sonosthesia.Generator
{
    public class GeneratorSignal<TValue, TProcessor> : StatelessSignal<TValue> where TValue : struct where TProcessor : IProcessor<TValue>
    {
        [SerializeField] private Generator<TValue> _generator;

        [SerializeField] private InterfaceReference<ISignal<float>> _timeSignal;

        [SerializeField] private TProcessor _processor;
        
        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _timeSignal.Value?.Observable.Subscribe(time =>
            {
                TValue raw = _generator.Evaluate(time);
                Broadcast(_processor.Process(raw));
            });
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