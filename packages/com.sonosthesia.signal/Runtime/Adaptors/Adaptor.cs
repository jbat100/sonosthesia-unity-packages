using System;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public abstract class Adaptor<TSource, TTarget> : Signal<TTarget> where TSource : struct where TTarget : struct
    {
        [SerializeField] private Signal<TSource> _source;

        private IDisposable _subscription;

        protected virtual IDisposable Setup(Signal<TSource> source) => null;
        
        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = Setup(_source);
        }
        
        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}