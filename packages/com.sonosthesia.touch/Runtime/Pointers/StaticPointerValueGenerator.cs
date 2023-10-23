using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    public class StaticPointerValueGenerator<TValue> : PointerValueGenerator<TValue> where TValue : struct
    {
        [SerializeField] private TValue _value;
        
        public sealed override bool OnPointerDown(PointerEventData eventData, out TValue value)
        {
            value = _value;
            return true;
        }

        public sealed override bool OnPointerMove(PointerEventData eventData, out TValue value)
        {
            value = _value;
            return true;
        }

        public sealed override void OnPointerEnd(PointerEventData eventData)
        {
            
        }
    }
}