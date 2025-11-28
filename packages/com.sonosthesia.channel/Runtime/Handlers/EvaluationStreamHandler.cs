using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Channel
{
    public abstract class EvaluationStreamHandler<T> : StreamHandler<T> where T : struct
    {
        // then use target with signals 
        [SerializeField] private InterfaceReference<ISignal<float>> _signal;

        private T? firstValue;

        protected abstract float Evaluate(T first, T last);
        
        protected override IDisposable InternalHandleStream(IObservable<T> stream)
        {
            return stream.Subscribe(value =>
            {
                firstValue ??= value;
                _signal.Value?.Broadcast(Evaluate(firstValue.Value, value));
            });
        }

        protected override void Complete()
        {
            base.Complete();
            firstValue = default;
        }
    }
}