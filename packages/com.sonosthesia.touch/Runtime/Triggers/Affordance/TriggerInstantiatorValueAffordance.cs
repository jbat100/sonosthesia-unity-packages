using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TriggerInstantiatorValueAffordance<TValue, TContainer> : 
        InstantiatorValueAffordance<TValue, TriggerValueEvent<TValue>, TContainer>
        where TValue : struct
        where TContainer : MonoBehaviour, IValueEventStreamContainer<TValue, TriggerValueEvent<TValue>>
    {
        
    }
}