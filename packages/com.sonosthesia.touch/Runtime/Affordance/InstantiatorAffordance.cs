using UnityEngine;

namespace Sonosthesia.Touch
{
    public class InstantiatorAffordance<TValue, TEvent, TSource> : Affordance<TValue, TEvent, TSource>
        where TValue : struct
        where TEvent : struct, IEventValue<TValue>
        where TSource : MonoBehaviour, IValueStreamSource<TValue, TEvent>
    {
        [SerializeField] private GameObject _prefab;
        
        
    }
}