using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Pointer
{
    public abstract class RelativePointerValueGenerator<TValue> : PointerValueGenerator<TValue> where TValue : struct
    {
        [SerializeField] private bool _track;
        
        [SerializeField] private bool _relative;

        private readonly Dictionary<int, TValue> _initials = new ();

        protected virtual TValue PostProcessValue(TValue value) => value;

        public sealed override bool OnPointerDown(PointerEventData eventData, out TValue value)
        {
            if (Extract(eventData, out TValue extracted))
            {
                _initials[eventData.pointerId] = extracted;
                value = _relative ? Relative(extracted, extracted) : extracted;
                value = PostProcessValue(value);
                return true;
            }
            value = default;
            return false;
        }

        public sealed override bool OnPointerMove(PointerEventData eventData, out TValue value)
        {
            if (_initials.TryGetValue(eventData.pointerId, out TValue initial))
            {
                if (!_track)
                {
                    value = initial;
                    return true;
                }
                if (Extract(eventData, out TValue extracted))
                {
                    value = _relative ? Relative(initial, extracted) : extracted;
                    value = PostProcessValue(value);
                    return true;
                }
            }
            value = default;
            return false;
        }

        public sealed override void OnPointerEnd(PointerEventData eventData)
        {
            _initials.Remove(eventData.pointerId);
        }

        protected abstract TValue Relative(TValue initial, TValue current);
        
        protected abstract bool Extract(PointerEventData eventData, out TValue value);
    }
}