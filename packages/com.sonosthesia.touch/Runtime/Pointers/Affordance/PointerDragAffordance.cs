using System;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class PointerDragAffordance : DragAffordance<PointerSourceEvent, BasePointerSource, PointerDragAffordance>
    {
        [SerializeField] private float _offset = 0.1f;
        
        protected new class Controller : DragAffordance<PointerSourceEvent, BasePointerSource, PointerDragAffordance>.Controller
        {
            public Controller(Guid id, PointerDragAffordance affordance) : base (id, affordance)
            {
                
            }
            
            private Camera _camera;
            
            protected override bool GetOriginPosition(bool initial, PointerSourceEvent value, ref Vector3 origin)
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

            protected override bool GetTargetPosition(bool initial, PointerSourceEvent value, Vector3 origin, ref Vector3 target)
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
        }

        protected override IObserver<PointerSourceEvent> MakeController(Guid id)
        {
            return new Controller(id, this);
        }
    }
}