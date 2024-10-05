using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class RelativeTriggerValueGenerator <TValue> : TriggerValueGenerator<TValue> where TValue : struct
    {
        [SerializeField] private bool _track;
        
        [SerializeField] private bool _relative;

        private readonly Dictionary<ITriggerData, TValue> _initials = new ();

        protected virtual TValue PostProcess(TValue value) => value;
        
        protected abstract TValue Relative(TValue initial, TValue current);
        
        protected abstract bool Extract(ITriggerData triggerData, out TValue value);
        
        public sealed override bool BeginTrigger(ITriggerData triggerData, out TValue value)
        {
            if (Extract(triggerData, out TValue extracted))
            {
                _initials[triggerData] = extracted;
                value = _relative ? Relative(extracted, extracted) : extracted;
                value = PostProcess(value);
                return true;
            }
            value = default;
            return false;
        }

        public sealed override bool UpdateTrigger(ITriggerData triggerData, out TValue value)
        {
            if (_initials.TryGetValue(triggerData, out TValue initial))
            {
                if (!_track)
                {
                    value = initial;
                    return true;
                }
                if (Extract(triggerData, out TValue extracted))
                {
                    value = _relative ? Relative(initial, extracted) : extracted;
                    value = PostProcess(value);
                    return true;
                }
            }
            value = default;
            return false;
        }

        public sealed override void EndTrigger(ITriggerData triggerData)
        {
            _initials.Remove(triggerData);
        }
    }
}