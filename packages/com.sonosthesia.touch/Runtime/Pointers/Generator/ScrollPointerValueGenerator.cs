using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    public class ScrollPointerValueGenerator : StatefulPointerValueGenerator<float, ScrollPointerValueGenerator.State>
    {
        public class State
        {
            public float CumulativeScroll;
        }

        [SerializeField] private float _sensitivity = 1f;

        [SerializeField] private FloatProcessor _postProcessor;

        protected override bool BeginPointer(PointerEventData eventData, State state, out float value)
        {
            value = 0;
            return true;
        }

        protected override bool UpdatePointer(PointerEventData eventData, State state, float initial, float previous, out float value)
        {
            if (eventData.scrollDelta.y != 0)
            {
                state.CumulativeScroll += eventData.scrollDelta.y;    
            }
            value = _sensitivity * state.CumulativeScroll;
            return true;
        }

        protected override float PostProcess(float value) => _postProcessor.Process(value);

        protected override float Relative(float initial, float current) => current - initial;
    }
}