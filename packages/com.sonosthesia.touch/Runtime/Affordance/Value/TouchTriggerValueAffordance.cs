using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchTriggerValueAffordance<TValue, TContainer> : 
        TriggerValueAffordance<TValue, TriggerValueEvent<TValue>, TContainer>
        where TValue : struct
        where TContainer : MonoBehaviour, IValueEventStreamContainer<TValue, TriggerValueEvent<TValue>>
    {
        
    }
}