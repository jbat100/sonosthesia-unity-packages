using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public abstract class Merger<T1, T2, TResult> : Signal<TResult> where T1 : struct where T2 : struct where TResult : struct
    {
        [SerializeField] private Signal<T1> _first;

        [SerializeField] private Signal<T2> _second;

        private IDisposable _subscription;

        protected abstract TResult Combine(T1 first, T2 second);
        
        protected void OnEnable()
        {
            _subscription?.Dispose();
            if (_first && _second)
            {
                _subscription = _first.SignalObservable
                    .CombineLatest(_second.SignalObservable, (f, s) => new KeyValuePair<T1, T2>(f, s))
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