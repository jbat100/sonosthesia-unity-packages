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
        
        [SerializeField] private bool _relative;
        
        private readonly Dictionary<ITriggerData, History> _history = new ();
        
        public sealed override bool BeginTrigger(ITriggerData triggerData, out TValue value)
        {
            TState state = new TState();
            if (!BeginTrigger(triggerData, state, out value))
            {
                return false;
            }
            _history[triggerData] = new History(value, state);
            value = _relative ? Relative(value, value) : value;
            value = PostProcess(value);
            return true;
        }

        public sealed override bool UpdateTrigger(ITriggerData triggerData, out TValue value)
        {
            if (_history.TryGetValue(triggerData, out History history))
            {
                if (!_track)
                {
                    value = history.InitialValue;
                    value = _relative ? Relative(value, value) : value;
                    value = PostProcess(value);
                    return true;
                }
                if (UpdateTrigger(triggerData, history.State, history.InitialValue, history.PreviousValue, out value))
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

        public sealed override void EndTrigger(ITriggerData triggerData)
        {
            _history.Remove(triggerData);
        }

        protected abstract bool BeginTrigger(ITriggerData triggerData, TState state, out TValue value);

        protected abstract bool UpdateTrigger(ITriggerData triggerData, TState state, TValue initial, TValue previous, out TValue value);
        
        protected abstract TValue Relative(TValue initial, TValue current);
        
        protected virtual TValue PostProcess(TValue value) => value;
    }
}