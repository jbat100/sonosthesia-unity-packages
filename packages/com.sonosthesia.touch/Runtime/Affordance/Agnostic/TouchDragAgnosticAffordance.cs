using System;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchDragAgnosticAffordance : DragAgnosticAffordance<TouchEvent, TouchEndpoint, TouchDragAgnosticAffordance>
    {
        protected new class Controller : DragAgnosticAffordance<TouchEvent, TouchEndpoint, TouchDragAgnosticAffordance>.Controller
        {
            public Controller(Guid eventId, TouchDragAgnosticAffordance affordance) : base (eventId, affordance)
            {
                
            }

            protected override bool GetOriginScale(bool initial, TouchEvent value, ref Vector3 origin) => false;
            
            protected override bool GetTargetScale(bool initial, TouchEvent value, Vector3 origin, ref Vector3 target) => false;

            protected override bool GetOriginPosition(bool initial, TouchEvent value, ref Vector3 origin)
            {
                origin = value.TouchData.Source.transform.position;
                return true;
            }

            protected override bool GetTargetPosition(bool initial, TouchEvent value, Vector3 origin, ref Vector3 target)
            {
                target = value.TouchData.Actor.transform.position;
                return true;
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id)
        {
            return new Controller(id, this);
        }
    }
}