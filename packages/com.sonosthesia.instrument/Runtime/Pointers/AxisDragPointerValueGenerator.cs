using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Instrument
{
    public class AxisDragPointerValueGenerator : StatefulPointerValueGenerator<EmptyPointerState, float>
    {
        [SerializeField] private Axes _dragAxis;

        [SerializeField] private float _scale;

        [SerializeField] private float _offset;

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

            Vector3 drag = _relative ?
                eventData.pointerCurrentRaycast.worldPosition - eventData.pointerPressRaycast.worldPosition :
                eventData.pointerCurrentRaycast.worldPosition;

            drag = transform.InverseTransformPoint(drag).FilterAxes(_dragAxis);
            value = _offset + drag.SumComponents() * _scale;
            return true;
        }
    }
}