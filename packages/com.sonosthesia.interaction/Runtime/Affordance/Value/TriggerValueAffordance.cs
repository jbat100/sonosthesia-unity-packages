using System;
using Sonosthesia.Trigger;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class TriggerValueAffordance<TValue, TEvent, TContainer> : ValueAffordance<TValue, TEvent, TContainer> 
        where TValue : struct
        where TEvent : struct, IValueEvent<TValue>
        where TContainer : MonoBehaviour, IValueEventStreamContainer<TValue, TEvent>
    {
        [SerializeField] private ValueTrigger<TValue> trigger;
        
        protected override void HandleStream(Guid id, IObservable<TEvent> stream)
        {
            base.HandleStream(id, stream);
            stream.Take(1).Subscribe(e =>
            {
                trigger.Trigger(e.GetValue());
            });
        }
    }
}