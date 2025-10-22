using System;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchDragAffordance : DragAffordance<TouchEvent>
    {
        private class Controller : DragAffordanceController<TouchEvent, TouchDragAffordance>
        {
            public Controller(Guid eventId, TouchDragAffordance affordance) : base (eventId, affordance)
            {
                
            }

            protected override bool GetOriginScale(bool initial, TouchEvent value, ref Vector3 origin) => false;
            
            protected override bool GetTargetScale(bool initial, TouchEvent value, Vector3 origin, ref Vector3 target) => false;

            protected override bool GetOriginPosition(bool initial, TouchEvent value, ref Vector3 origin)
            {
                origin = value.touchData.Source.transform.position;
                return true;
            }

            protected override bool GetTargetPosition(bool initial, TouchEvent value, Vector3 origin, ref Vector3 target)
            {
                target = value.touchData.Actor.transform.position;
                return true;
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return new Controller(id, this);
        }
    }
}