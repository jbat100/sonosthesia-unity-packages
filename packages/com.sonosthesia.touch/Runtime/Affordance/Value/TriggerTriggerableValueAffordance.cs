using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    /// <summary>
    /// A bit off a silly name but:
    /// - Trigger refers to unity physics Collider Trigger
    /// - Triggerable refers to com.sonosthesia.trigger Triggerables
    /// </summary>
    public class TriggerTriggerableValueAffordance<TValue, TContainer> : 
        TriggerableValueAffordance<TValue, TriggerValueEvent<TValue>, TContainer>
        where TValue : struct
        where TContainer : MonoBehaviour, IValueEventStreamContainer<TValue, TriggerValueEvent<TValue>>
    {
        
    }
}