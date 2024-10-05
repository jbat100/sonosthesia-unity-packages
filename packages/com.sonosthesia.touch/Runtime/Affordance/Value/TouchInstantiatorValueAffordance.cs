using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchInstantiatorValueAffordance<TValue, TContainer> : 
        InstantiatorValueAffordance<TValue, TriggerValueEvent<TValue>, TContainer>
        where TValue : struct
        where TContainer : MonoBehaviour, IValueEventStreamContainer<TValue, TriggerValueEvent<TValue>>
    {
        
    }
}