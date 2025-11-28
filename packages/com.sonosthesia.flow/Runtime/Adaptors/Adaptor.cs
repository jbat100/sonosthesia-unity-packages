using System;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Flow
{
    public abstract class Adaptor<TSource, TTarget> : StatelessSignal<TTarget> where TSource : struct where TTarget : struct
    {
        [SerializeField] private InterfaceReference<ISignal<TSource>> _source;

        private IDisposable _subscription;

        protected virtual IDisposable Setup(ISignal<TSource> source) => null;
        
        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_source.Value != null)
            {
                _subscription = Setup(_source.Value);   
            }
        }
        
        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}