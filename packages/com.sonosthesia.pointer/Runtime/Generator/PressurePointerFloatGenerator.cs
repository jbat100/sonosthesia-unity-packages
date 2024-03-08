using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Pointer
{
    public class PressurePointerFloatGenerator : RelativePointerValueGenerator<float>
    {
        [SerializeField] private FloatProcessor _postProcessor;
        
        protected override float Relative(float initial, float current) => current - initial;

        protected override bool Extract(PointerEventData eventData, out float value)
        {
            Debug.Log($"{this} {nameof(Extract)} pressure {eventData.pressure}");
            value = eventData.pressure;
            return true;
        }

        protected override float PostProcessValue(float value) => _postProcessor.Process(value);
    }
}