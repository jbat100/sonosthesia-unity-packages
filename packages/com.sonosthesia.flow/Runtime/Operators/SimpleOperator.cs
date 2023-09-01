using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class SimpleOperator<T> : Operator<T> where T : struct
    {
        [SerializeField] private bool _bypass;
        
        protected override IDisposable Setup(Signal<T> source)
        {
            return source.SignalObservable.Subscribe(value =>
            {
                Broadcast(_bypass ? value : Process(value));
            });
        }

        protected abstract T Process(T input);
    }
}