using UnityEngine;

namespace Sonosthesia.Touch
{
    public class Affordance<TValue, TEvent, TSource> : MonoBehaviour 
        where TValue : struct
        where TEvent : struct, IEventValue<TValue>
        where TSource : MonoBehaviour, IValueStreamSource<TValue, TEvent>
    {
        
    }
}