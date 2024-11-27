using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Touch
{
    /// <summary>
    /// Allows expensive operation like Component extractions to be performed only once without committing the
    /// entire TriggerChannelSource to requiring a given component. It is up to StatefulTriggerValueGenerator
    /// subclasses to defined their State type and populate/update it according to its needs
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class StatefulTouchValueGenerator<TState, TValue> : TouchValueGenerator<TValue>
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
        
        [SerializeField] private bool _relative;
        
        private readonly Dictionary<ITouchData, History> _history = new ();
        
        public sealed override bool BeginTouch(ITouchData touchData, out TValue value)
        {
            TState state = new TState();
            if (!BeginTrigger(touchData, state, out value))
            {
                return false;
            }
            _history[touchData] = new History(value, state);
            value = _relative ? Relative(value, value) : value;
            value = PostProcess(value);
            return true;
        }

        public sealed override bool UpdateTouch(ITouchData touchData, out TValue value)
        {
            if (_history.TryGetValue(touchData, out History history))
            {
                if (!_track)
                {
                    value = history.InitialValue;
                    value = _relative ? Relative(value, value) : value;
                    value = PostProcess(value);
                    return true;
                }
                if (UpdateTrigger(touchData, history.State, history.InitialValue, history.PreviousValue, out value))
                {
                    history.PreviousValue = value;
                    value = _relative ? Relative(history.InitialValue, value) : value;
                    value = PostProcess(value);
                    return true;
                }
            }
            value = default;
            return false;
        }

        public sealed override void EndTouch(ITouchData touchData)
        {
            if (touchData == null)
            {
                Debug.LogWarning($"{this} got null trigger data, history has {_history.Count} items");
                return;
            }
            _history.Remove(touchData);
        }

        protected abstract bool BeginTrigger(ITouchData touchData, TState state, out TValue value);

        protected abstract bool UpdateTrigger(ITouchData touchData, TState state, TValue initial, TValue previous, out TValue value);
        
        protected abstract TValue Relative(TValue initial, TValue current);
        
        protected virtual TValue PostProcess(TValue value) => value;
    }
}