using System;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TriggerDragAffordance : DragAffordance<TriggerSourceEvent, BaseTriggerSource, TriggerDragAffordance>
    {
        protected new class Controller : DragAffordance<TriggerSourceEvent, BaseTriggerSource, TriggerDragAffordance>.Controller
        {
            public Controller(Guid id, TriggerDragAffordance affordance) : base (id, affordance)
            {
                
            }

            protected override bool GetOriginScale(bool initial, TriggerSourceEvent value, ref Vector3 origin)
            {
                return false;
            }

            protected override bool GetTargetScale(bool initial, TriggerSourceEvent value, Vector3 origin, ref Vector3 target)
            {
                return false;
            }
            
            protected override bool GetOriginPosition(bool initial, TriggerSourceEvent value, ref Vector3 origin)
            {
                origin = value.TriggerData.Source.transform.position;
                return true;
            }

            protected override bool GetTargetPosition(bool initial, TriggerSourceEvent value, Vector3 origin, ref Vector3 target)
            {
                target = value.TriggerData.Actor.transform.position;
                return true;
            }
        }

        protected override IObserver<TriggerSourceEvent> MakeController(Guid id)
        {
            return new Controller(id, this);
        }
    }
}