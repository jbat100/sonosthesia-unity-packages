using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    public abstract class StatefulPointerValueGenerator<TValue, TState> : PointerValueGenerator<TValue> 
        where TValue : struct where TState : class, new()
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
        
        private readonly Dictionary<int, History> _history = new ();

        protected abstract bool BeginPointer(PointerEventData eventData, TState state, out TValue value);

        protected abstract bool UpdatePointer(PointerEventData eventData, TState state, TValue initial, TValue previous, out TValue value);
        
        protected abstract TValue Relative(TValue initial, TValue current);
        
        protected virtual TValue PostProcess(TValue value) => value;
        
        public override bool OnPointerDown(PointerEventData eventData, out TValue value)
        {
            TState state = new TState();
            if (!BeginPointer(eventData, state, out value))
            {
                return false;
            }
            _history[eventData.pointerId] = new History(value, state);
            value = _relative ? Relative(value, value) : value;
            value = PostProcess(value);
            return true;
        }

        public override bool OnPointerMove(PointerEventData eventData, out TValue value)
        {
            if (_history.TryGetValue(eventData.pointerId, out History history))
            {
                if (!_track)
                {
                    value = history.InitialValue;
                    value = _relative ? Relative(value, value) : value;
                    value = PostProcess(value);
                    return true;
                }
                if (UpdatePointer(eventData, history.State, history.InitialValue, history.PreviousValue, out value))
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

        public override void OnPointerEnd(PointerEventData eventData)
        {
            _history.Remove(eventData.pointerId);
        }
    }
}