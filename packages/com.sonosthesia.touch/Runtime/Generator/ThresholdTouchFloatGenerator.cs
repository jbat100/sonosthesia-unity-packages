using System;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class ThresholdTouchFloatGenerator : StatefulTouchValueGenerator<ThresholdTouchFloatGenerator.State, float>
    {
        public class State
        {
            // this is the raw initial value with no threshold applied
            internal float Initial { get; set; }

            internal float? Reference { get; set; }
        }

        public enum ThresholdType
        {
            None,
            Positive,
            Negative,
            Absolute
        }

        [SerializeField] private float _threshold;

        [SerializeField] private ThresholdType _thresholdType;
        
        protected abstract bool Extract(ITouchData touchData, out float value);
        
        protected override bool BeginTrigger(ITouchData touchData, State state, out float value)
        {
            value = 0;
            if (Extract(touchData, out float extracted))
            {
                state.Initial = extracted;
                return true;
            }
            return false;
        }

        protected override bool UpdateTrigger(ITouchData touchData, State state, float initial, float previous, out float value)
        {
            value = 0;
            if (Extract(touchData, out float extracted))
            {
                if (state.Reference.HasValue)
                {
                    value = extracted - state.Reference.Value;
                }
                else if (CheckThreshold(extracted))
                {
                    state.Reference = extracted;
                }
                return true;
            }
            return false;
        }

        private bool CheckThreshold(float value) => _thresholdType switch
        {
            ThresholdType.Positive => value > _threshold,
            ThresholdType.Negative => value < _threshold,
            ThresholdType.Absolute => Mathf.Abs(value) > _threshold,
            _ => false
        };
    }
}