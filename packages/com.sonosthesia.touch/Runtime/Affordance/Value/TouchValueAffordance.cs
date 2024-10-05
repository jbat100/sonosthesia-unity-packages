using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchValueAffordance<TValue, TContainer> : ValueAffordance<TValue, TriggerValueEvent<TValue>, TContainer>
        where TValue : struct
        where TContainer : MonoBehaviour, IValueEventStreamContainer<TValue, TriggerValueEvent<TValue>>
    {
        
    }
}