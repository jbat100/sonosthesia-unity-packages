using System;
using UnityEngine;
using Sonosthesia.Signal;
using Sonosthesia.Utils;

namespace Sonosthesia.Flow
{
    public abstract class Adaptor<TSource, TTarget> : MonoBehaviour where TSource : struct where TTarget : struct
    {
        [SerializeField] private InterfaceReference<ISignal<TSource>> _source;
        
        [SerializeField] private InterfaceReference<ISignal<TTarget>> _target;

        private IDisposable _subscription;

        protected virtual IDisposable Setup(ISignal<TSource> source) => null;
        
        protected void Broadcast(TTarget value) => _target.Value?.Broadcast(value);
        
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