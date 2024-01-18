using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    public class AxisPointerFloatGenerator : RelativePointerValueGenerator<float>
    {
        private enum AxisType
        {
            Raycast,
            Position
        }
        
        [SerializeField] private Vector3ToFloat _extractor;

        [SerializeField] private AxisType _axisType;

        [SerializeField] private bool _applyTransferCurve;
        
        [SerializeField] private AnimationCurve _transferCurve;

        [SerializeField] private FloatProcessor _postProcessor;

        private abstract class Implementation
        {
            public abstract bool Extract(Transform transform, PointerEventData eventData, out Vector3 value);
        }

        private class RaycastImplementation : Implementation
        {
            public override bool Extract(Transform transform, PointerEventData eventData, out Vector3 value)
            {
                if (!eventData.pointerCurrentRaycast.gameObject || 
                    !eventData.pointerPressRaycast.gameObject ||
                    eventData.pointerPressRaycast.gameObject != eventData.pointerCurrentRaycast.gameObject)
                {
                    value = default;
                    return false;
                }
                value = transform.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition);
                return true;
            }
        }

        private class PositionImplementation : Implementation
        {
            public override bool Extract(Transform transform, PointerEventData eventData, out Vector3 value)
            {
                value = eventData.position;
                return true;
            }
        }
        
        private Implementation _implementation;

        private void RefreshImplementation()
        {
            _implementation = _axisType switch
            {
                AxisType.Raycast => new RaycastImplementation(),
                AxisType.Position => new PositionImplementation(),
                _ => new PositionImplementation()
            };
        }

        protected virtual void OnEnable() => RefreshImplementation();
        
        protected virtual void OnValidate() => RefreshImplementation();

        protected override float Relative(float initial, float current) => current - initial;

        protected override bool Extract(PointerEventData eventData, out float value)
        {
            if (_implementation != null && _implementation.Extract(transform, eventData, out Vector3 axis))
            {
                value = _extractor.ExtractFloat(axis);
                if (_applyTransferCurve)
                {
                    value = _transferCurve.Evaluate(value);
                }
                return true;
            }
            value = default;
            return false;
        }

        protected override float PostProcessValue(float value)
        {
            return _postProcessor.Process(value);
        }
    }
}