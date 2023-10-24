using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Channel
{
    public abstract class ChannelStreamSignalHandler<T> : ChannelStreamHandler<T> where T : struct
    {
        // then use target with signals 
        [SerializeField] private Signal<float> _signal;

        private T? firstValue;

        protected abstract float Evaluate(T first, T last);
        
        protected override IDisposable InternalHandleStream(IObservable<T> stream)
        {
            return stream.Subscribe(value =>
            {
                firstValue ??= value;
                _signal.Broadcast(Evaluate(firstValue.Value, value));
            });
        }

        protected override void Complete()
        {
            base.Complete();
            firstValue = default;
        }
    }
}