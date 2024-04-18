using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Flow
{
    public abstract class Mixer<TValue> : Signal<TValue> where TValue : struct
    {
        [SerializeField] private List<Signal<TValue>> _inputs;

        // TODO : could use signal.Value instead of storing in dictionary
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
            foreach (Signal<TValue> input in _inputs.Where(input => input))
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