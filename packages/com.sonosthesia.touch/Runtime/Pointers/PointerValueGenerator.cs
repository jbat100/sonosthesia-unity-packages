using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    public abstract class PointerValueGenerator<TValue> : MonoBehaviour where TValue : struct
    {
        public abstract bool OnPointerDown(PointerEventData eventData, out TValue value);

        public abstract bool OnPointerMove(PointerEventData eventData, out TValue value);
        
        public abstract void OnPointerEnd(PointerEventData eventData);
    }

    public static class PointerValueGeneratorExtensions
    {
        public static bool OnPointer<TValue>(this PointerValueGenerator<TValue> generator,
            bool initial, PointerEventData eventData, out TValue value) where TValue : struct
        {
            return initial ? 
                generator.OnPointerDown(eventData, out value) : 
                generator.OnPointerMove(eventData, out value);
        }
    }
}