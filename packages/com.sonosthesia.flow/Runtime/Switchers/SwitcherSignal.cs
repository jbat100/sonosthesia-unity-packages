using System;
using System.Collections.Generic;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Flow
{
    public class SwitcherSignal<TValue> : Signal<TValue> where TValue : struct
    {
        [SerializeField] private Signal<float> _signal;

        [SerializeField] private SafeIndex _safeIndex;
        
        [SerializeField] private List<TValue> _values;

        private IDisposable _subscription;

        private int? _current;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _signal.SignalObservable.Subscribe(value =>
            {
                int index = Mathf.RoundToInt(value);
                if (_current == index)
                {
                    return;
                }

                _current = index;

                if (_values.TryGetIndex(index, _safeIndex, out TValue selected))
                {
                    Broadcast(selected);
                }
            });
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
            _current = null;
        }
    }
}