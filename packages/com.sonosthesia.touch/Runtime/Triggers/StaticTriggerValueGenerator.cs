using UnityEngine;

namespace Sonosthesia.Touch
{
    public class StaticTriggerValueGenerator<TValue> : TriggerValueGenerator<TValue> where TValue : struct
    {
        [SerializeField] private TValue _value;
        
        public override bool ProcessTriggerEnter(Collider other, out TValue value)
        {
            value = _value;
            return true;
        }

        public override bool ProcessTriggerStay(Collider other, out TValue value)
        {
            value = _value;
            return true;
        }

        public override void ProcessTriggerExit(Collider other)
        {
            
        }
    }
}