using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class Target<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private Signal<T> _source;

        private IDisposable _subscription;

        protected virtual void Awake()
        {
            if (!_source)
            {
                _source = GetComponent<Signal<T>>();
            }
        }
        
        protected void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _source.SignalObservable.Subscribe(Apply);
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        protected abstract void Apply(T value);
    }
}