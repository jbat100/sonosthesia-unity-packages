using System;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Pointer
{
    public class PointerDragAffordance : DragAffordance<PointerEvent>
    {
        public enum ScaleDriver
        {
            None,
            Scroll,
            Pressure
        }
        
        [SerializeField] private float _offset = 0.1f;
        
        [SerializeField] private ScaleDriver _scaleDriver;
        
        [SerializeField] private float _scaleSensitivity = 0.1f;
        
        private class Controller : DragAffordanceController<PointerEvent, PointerDragAffordance>
        {
            public Controller(Guid eventId, PointerDragAffordance affordance) : base (eventId, affordance)
            {
                
            }
            
            private Camera _camera;
            private float _initialPressure;
            private Vector2 _cumulativeScroll;
            private Vector3 _initialTargetScale;

            protected override void Update(PointerEvent updatedEvent)
            {
                base.Update(updatedEvent);
                if (updatedEvent.Data.IsScrolling())
                {
                    _cumulativeScroll += updatedEvent.Data.scrollDelta;   
                }
            }

            protected override bool GetOriginPosition(bool initial, PointerEvent value, ref Vector3 origin)
            {
                if (!initial)
                {
                    return false;
                }
                
                _camera = Camera.main;
                Vector3 cameraPosition = _camera.transform.position;
                Vector3 look = value.Data.pointerCurrentRaycast.worldPosition - cameraPosition;
                float distance = look.magnitude;
                float offsetDistance = Mathf.Max(_camera.nearClipPlane, distance - Affordance._offset);
                
                origin = cameraPosition + look * (offsetDistance / distance);
                
                // not sure why I need to do this, I would have thought origin would be at screen value.Data.position
                Plane plane = new Plane(-value.Source.transform.forward, origin);
                Ray ray = _camera.ScreenPointToRay(value.Data.position);
                if (!plane.Raycast(ray, out float enter))
                {
                    return false;
                }
                origin = ray.GetPoint(enter);
                
                return true;
            }

            protected override bool GetTargetPosition(bool initial, PointerEvent value, Vector3 origin, ref Vector3 target)
            {
                Plane plane = new Plane(-value.Source.transform.forward, origin);
                Ray ray = _camera.ScreenPointToRay(value.Data.position);
                if (!plane.Raycast(ray, out float enter))
                {
                    return false;
                }
                target = ray.GetPoint(enter);
                
                return true;
            }

            protected override bool GetOriginScale(bool initial, PointerEvent value, ref Vector3 origin)
            {
                return false;
            }

            protected override bool GetTargetScale(bool initial, PointerEvent value, Vector3 origin, ref Vector3 target)
            {
                if (initial)
                {
                    _initialPressure = value.Data.pressure;
                    _initialTargetScale = target;
                    return false;
                }

                switch (Affordance._scaleDriver)
                {
                    case ScaleDriver.Pressure:
                        target = _initialTargetScale * Affordance._scaleSensitivity * (value.Data.pressure - _initialPressure);
                        return true;
                    case ScaleDriver.Scroll:
                        target = _initialTargetScale * Affordance._scaleSensitivity * _cumulativeScroll.y;
                        return true;
                }

                return false;
            }
        }

        protected override IObserver<PointerEvent> MakeController(Guid id)
        {
            return new Controller(id, this);
        }
    }
}