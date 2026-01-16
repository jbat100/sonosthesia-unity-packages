using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Sonosthesia.Signal;
using Sonosthesia.Utils;

namespace Sonosthesia.Flow
{
    public abstract class Merger<T1, T2, TResult> : StatelessSignal<TResult> where T1 : struct where T2 : struct where TResult : struct
    {
        [SerializeField] private InterfaceReference<ISignal<T1>> _first;

        [SerializeField] private InterfaceReference<ISignal<T2>> _second;

        private IDisposable _subscription;

        protected abstract TResult Combine(T1 first, T2 second);
        
        protected void OnEnable()
        {
            _subscription?.Dispose();
            if (_first.Value != null && _second.Value != null)
            {
                _subscription = _first.Value.Observable
                    .CombineLatest(_second.Value.Observable, (f, s) => new KeyValuePair<T1, T2>(f, s))
                    .Subscribe(pair => Combine(pair.Key, pair.Value));
            }
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}