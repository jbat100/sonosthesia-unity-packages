using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class RelativeTouchValueGenerator <TValue> : TouchValueGenerator<TValue> where TValue : struct
    {
        [SerializeField] private bool _track;
        
        [SerializeField] private bool _relative;

        private readonly Dictionary<ITouchData, TValue> _initials = new ();

        protected virtual TValue PostProcess(TValue value) => value;
        
        protected abstract TValue Relative(TValue initial, TValue current);
        
        protected abstract bool Extract(ITouchData touchData, out TValue value);
        
        public sealed override bool BeginTouch(ITouchData touchData, out TValue value)
        {
            if (Extract(touchData, out TValue extracted))
            {
                _initials[touchData] = extracted;
                value = _relative ? Relative(extracted, extracted) : extracted;
                value = PostProcess(value);
                return true;
            }
            value = default;
            return false;
        }

        public sealed override bool UpdateTouch(ITouchData touchData, out TValue value)
        {
            if (_initials.TryGetValue(touchData, out TValue initial))
            {
                if (!_track)
                {
                    value = initial;
                    return true;
                }
                if (Extract(touchData, out TValue extracted))
                {
                    value = _relative ? Relative(initial, extracted) : extracted;
                    value = PostProcess(value);
                    return true;
                }
            }
            value = default;
            return false;
        }

        public sealed override void EndTouch(ITouchData touchData)
        {
            _initials.Remove(touchData);
        }
    }
}