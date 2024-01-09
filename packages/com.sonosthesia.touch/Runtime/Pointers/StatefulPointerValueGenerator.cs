using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    public abstract class StatefulPointerValueGenerator<TValue> : PointerValueGenerator<TValue> where TValue : struct
    {
        private class History
        {
            public TValue InitialValue { get; }
            public TValue PreviousValue { get; set; }

            public History(TValue initial)
            {
                InitialValue = initial;
                PreviousValue = initial;
            }
        }

        [SerializeField] private bool _track;
        
        [SerializeField] private bool _relative;

        private readonly Dictionary<int, History> _history = new ();

        protected virtual TValue PostProcessValue(TValue value)
        {
            return value;
        }

        public sealed override bool OnPointerDown(PointerEventData eventData, out TValue value)
        {
            if (Extract(eventData, out TValue extracted))
            {
                _history[eventData.pointerId] = new History(extracted);
                value = _relative ? Relative(extracted, extracted) : extracted;
                value = PostProcessValue(value);
                return true;
            }
            value = default;
            return false;
        }

        public sealed override bool OnPointerMove(PointerEventData eventData, out TValue value)
        {
            if (_history.TryGetValue(eventData.pointerId, out History history))
            {
                if (!_track)
                {
                    value = history.InitialValue;
                    return true;
                }
                if (Extract(eventData, out TValue extracted))
                {
                    history.PreviousValue = extracted;
                    value = _relative ? Relative(history.InitialValue, extracted) : extracted;
                    value = PostProcessValue(value);
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

        protected abstract TValue Relative(TValue initial, TValue current);
        
        protected abstract bool Extract(PointerEventData eventData, out TValue value);
    }
}