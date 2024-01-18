using UnityEngine;

namespace Sonosthesia.Touch
{
    public class StaticTriggerValueGenerator<TValue> : TriggerValueGenerator<TValue> where TValue : struct
    {
        [SerializeField] private TValue _value;
        
        public override bool BeginTrigger(ITriggerData triggerData, out TValue value)
        {
            value = _value;
            return true;
        }

        public override bool UpdateTrigger(ITriggerData triggerData, out TValue value)
        {
            value = _value;
            return true;
        }

        public override void EndTrigger(ITriggerData triggerData)
        {
            
        }
    }
}