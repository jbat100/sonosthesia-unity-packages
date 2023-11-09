using System;
using System.Collections.Generic;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Flow
{
    public abstract class Switcher<T> : MonoBehaviour where T : class
    {
        [SerializeField] private Signal<float> _signal;

        [SerializeField] private SafeIndex _safeIndex;
        
        [SerializeField] private List<T> _targets;

        private IDisposable _subscription;
        
        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _signal.SignalObservable.Subscribe(value =>
            {
                _targets.TryGetIndex(value, _safeIndex, out T selected);
                foreach (T target in _targets)
                {
                    Switch(target, target == selected);
                }
            });
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        protected abstract void Switch(T target, bool on);
    }
}