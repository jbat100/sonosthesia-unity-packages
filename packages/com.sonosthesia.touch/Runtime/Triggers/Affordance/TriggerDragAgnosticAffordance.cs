using System;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TriggerDragAgnosticAffordance : DragAgnosticAffordance<TriggerEvent, TriggerEndpoint, TriggerDragAgnosticAffordance>
    {
        protected new class Controller : DragAgnosticAffordance<TriggerEvent, TriggerEndpoint, TriggerDragAgnosticAffordance>.Controller
        {
            public Controller(Guid id, TriggerDragAgnosticAffordance affordance) : base (id, affordance)
            {
                
            }

            protected override bool GetOriginScale(bool initial, TriggerEvent value, ref Vector3 origin)
            {
                return false;
            }

            protected override bool GetTargetScale(bool initial, TriggerEvent value, Vector3 origin, ref Vector3 target)
            {
                return false;
            }
            
            protected override bool GetOriginPosition(bool initial, TriggerEvent value, ref Vector3 origin)
            {
                origin = value.TriggerData.Source.transform.position;
                return true;
            }

            protected override bool GetTargetPosition(bool initial, TriggerEvent value, Vector3 origin, ref Vector3 target)
            {
                target = value.TriggerData.Actor.transform.position;
                return true;
            }
        }

        protected override IObserver<TriggerEvent> MakeController(Guid id)
        {
            return new Controller(id, this);
        }
    }
}