using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class StatefulTriggerValueGenerator<TState, TValue> : TriggerValueGenerator<TValue>
        where TState : class, new() where TValue : struct
    {
        private class History
        {
            public TValue InitialValue { get; set; }
            public TValue PreviousValue { get; set; }
            public TState State { get; }

            public History(TValue initial, TState state)
            {
                InitialValue = initial;
                PreviousValue = initial;
                State = state;
            }
        }

        [SerializeField] private bool _track;
        
        private readonly Dictionary<Collider, History> _history = new ();
        
        public sealed override bool ProcessTriggerEnter(Collider other, out TValue value)
        {
            TState state = new TState();
            if (ProcessTriggerEnter(other, state, out value))
            {
                _history[other] = new History(value, state);
                return true;
            }
            return false;
        }

        public sealed override bool ProcessTriggerStay(Collider other, out TValue value)
        {
            if (_history.TryGetValue(other, out History history))
            {
                if (_track)
                {
                    value = history.InitialValue;
                    return true;
                }
                if (ProcessTriggerStay(other, history.State, history.InitialValue, history.PreviousValue, out value))
                {
                    history.PreviousValue = value;
                    return true;
                }
            }
            value = default;
            return false;
        }

        public sealed override void ProcessTriggerExit(Collider other)
        {
            _history.Remove(other);
        }

        protected abstract bool ProcessTriggerEnter(Collider other, TState state, out TValue value);

        protected abstract bool ProcessTriggerStay(Collider other, TState state, TValue initial, TValue previous, out TValue value);
    }
}