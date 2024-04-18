using System;
using UniRx;
using Sonosthesia.Signal;

namespace Sonosthesia.Flow
{
    public abstract class SimpleOperator<T> : Operator<T> where T : struct
    {
        protected override IDisposable Setup(Signal<T> source)
        {
            if (!source)
            {
                return Disposable.Empty;
            }
            return source.SignalObservable.Subscribe(value =>
            {
                Broadcast(Bypass ? value : Process(value));
            });
        }

        protected abstract T Process(T input);
    }
}