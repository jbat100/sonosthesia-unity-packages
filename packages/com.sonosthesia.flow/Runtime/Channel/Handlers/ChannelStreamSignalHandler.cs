using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class ChannelStreamSignalHandler<T> : ChannelStreamHandler<T> where T : struct
    {
        // then use target with signals 
        [SerializeField] private DrivenFloatSignal _signal;

        private T firstValue;

        protected abstract float Evaluate(T first, T last);
        
        protected override IDisposable InternalHandleStream(IObservable<T> stream)
        {
            IDisposable startSubscription = stream.First().Subscribe(value =>
            {
                firstValue = value;
                _signal.Drive(Evaluate(firstValue, firstValue));
            });
            IDisposable mainSubscription = stream.Subscribe(value =>
            {
                _signal.Drive(Evaluate(firstValue, value));
            });
            return new CompositeDisposable {startSubscription, mainSubscription};
        }

        protected override void Complete()
        {
            base.Complete();
            firstValue = default;
        }
    }
}