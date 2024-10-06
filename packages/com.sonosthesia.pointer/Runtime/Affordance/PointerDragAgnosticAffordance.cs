using System;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Pointer
{
    public class PointerDragAgnosticAffordance : 
        DragAgnosticAffordance<PointerEvent, PointerEventStreamContainer, PointerDragAgnosticAffordance>
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
        
        protected new class Controller : 
            DragAgnosticAffordance<PointerEvent, PointerEventStreamContainer, PointerDragAgnosticAffordance>.Controller
        {
            public Controller(Guid eventId, PointerDragAgnosticAffordance affordance) : base (eventId, affordance)
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
                return true;
            }

            protected override bool GetTargetPosition(bool initial, PointerEvent value, Vector3 origin, ref Vector3 target)
            {
                Vector3 direction = _camera.transform.position - origin;

                //Plane plane = new Plane(direction.normalized, _originPosition);
                
                Plane plane = new Plane(-Affordance.transform.forward, origin);
                
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