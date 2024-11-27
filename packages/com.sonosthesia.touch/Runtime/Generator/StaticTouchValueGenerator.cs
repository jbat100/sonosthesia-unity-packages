using UnityEngine;

namespace Sonosthesia.Touch
{
    public class StaticTouchValueGenerator<TValue> : TouchValueGenerator<TValue> where TValue : struct
    {
        [SerializeField] private TValue _value;
        
        public override bool BeginTouch(ITouchData touchData, out TValue value)
        {
            value = _value;
            return true;
        }

        public override bool UpdateTouch(ITouchData touchData, out TValue value)
        {
            value = _value;
            return true;
        }

        public override void EndTouch(ITouchData touchData)
        {
            
        }
    }
}