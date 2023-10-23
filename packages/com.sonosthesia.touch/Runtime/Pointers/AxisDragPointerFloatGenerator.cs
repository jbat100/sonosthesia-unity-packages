using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    public class AxisDragPointerFloatGenerator : StatefulPointerValueGenerator<EmptyPointerState, float>
    {
        [SerializeField] private Vector3ToFloat _extractor;

        [SerializeField] private bool _relative;

        protected override bool OnPointerDown(PointerEventData eventData, EmptyPointerState state, out float value)
        {
            return ExtractValue(eventData, out value);
        }

        protected override bool OnPointerMove(PointerEventData eventData, EmptyPointerState state, float initial, float previous, out float value)
        {
            return ExtractValue(eventData, out value);
        }

        protected virtual bool ExtractValue(PointerEventData eventData, out float value)
        {
            if (!eventData.pointerCurrentRaycast.gameObject || 
                !eventData.pointerPressRaycast.gameObject ||
                eventData.pointerPressRaycast.gameObject != eventData.pointerCurrentRaycast.gameObject)
            {
                value = default;
                return false;
            }

            Vector3 localCurrent = transform.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition);
            Vector3 localPress = transform.InverseTransformPoint(eventData.pointerPressRaycast.worldPosition);
            Vector3 drag = _relative ? localCurrent - localPress : localCurrent;
            value = _extractor.Process(drag);
            return true;
        }
    }
}