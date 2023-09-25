using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class FloatMapAdaptor<TTarget> : Adaptor<float, TTarget> where TTarget : struct
    {
        [SerializeField] private float _offset = 0f;
        
        [SerializeField] private float _scale = 1f;

        protected abstract TTarget Map(float value);

        protected override IDisposable Setup(Signal<float> source)
        {
            return source.SignalObservable.Subscribe(value =>
            {
                TTarget mapped = Map(value * _scale + _offset);
                Broadcast(mapped);
            });
        }
    }
}