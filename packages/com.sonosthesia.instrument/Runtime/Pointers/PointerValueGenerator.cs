using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Instrument
{
    public abstract class PointerValueGenerator<TValue> : MonoBehaviour where TValue : struct
    {
        public abstract bool OnPointerDown(PointerEventData eventData, out TValue value);

        public abstract bool OnPointerMove(PointerEventData eventData, out TValue value);
        
        public abstract void OnPointerEnd(PointerEventData eventData);
    }
}