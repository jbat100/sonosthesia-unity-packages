using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Sonosthesia.Instrument
{
    public class EmptyPointerState { }
    
    public abstract class StatefulPointerValueGenerator<TState, TValue> : PointerValueGenerator<TValue>
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
        
        private readonly Dictionary<int, History> _history = new ();

        public sealed override bool OnPointerDown(PointerEventData eventData, out TValue value)
        {
            TState state = new TState();
            if (OnPointerDown(eventData, state, out value))
            {
                _history[eventData.pointerId] = new History(value, state);
                return true;
            }
            return false;
        }

        public sealed override bool OnPointerMove(PointerEventData eventData, out TValue value)
        {
            if (_history.TryGetValue(eventData.pointerId, out History history))
            {
                if (OnPointerMove(eventData, history.State, history.InitialValue, history.PreviousValue, out value))
                {
                    history.PreviousValue = value;
                    return true;
                }
            }
            value = default;
            return false;
        }

        public sealed override void OnPointerEnd(PointerEventData eventData)
        {
            _history.Remove(eventData.pointerId);
        }

        protected abstract bool OnPointerDown(PointerEventData eventData, TState state, out TValue value);

        protected abstract bool OnPointerMove(PointerEventData eventData, TState state, TValue initial, TValue previous, out TValue value);
    }
}