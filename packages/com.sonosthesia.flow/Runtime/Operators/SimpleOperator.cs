using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class SimpleOperator<T> : Operator<T> where T : struct
    {
        protected override IDisposable Setup(Signal<T> source)
        {
            return source.SignalObservable.Subscribe(value =>
            {
                Broadcast(Bypass ? value : Process(value));
            });
        }

        protected abstract T Process(T input);
    }
}