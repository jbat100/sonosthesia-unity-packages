using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public abstract class Mixer<TValue> : Signal<TValue> where TValue : struct
    {
        [SerializeField] private List<Signal<TValue>> _inputs;

        private readonly Dictionary<Signal<TValue>, TValue> _current = new();

        private readonly CompositeDisposable _subscriptions = new();

        private bool _dirty;

        protected abstract TValue Mix(IEnumerable<TValue> values);
        
        protected virtual void Update()
        {
            if (_dirty)
            {
                _dirty = false;
                Broadcast(Mix(_current.Values));
            }
        }
        
        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            foreach (Signal<TValue> input in _inputs)
            {
                _subscriptions.Add(input.SignalObservable.Subscribe(value =>
                {
                    _current[input] = value;
                    _dirty = true;
                }));
            }
        }

        protected virtual void OnDisable()
        {
            _subscriptions.Clear();
            _current.Clear();
        }
    }
}