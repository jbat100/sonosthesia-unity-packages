using System;
using Sonosthesia.Trigger;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class TriggerValueAffordance<TValue, TEvent> : ValueAffordance<TValue, TEvent> 
        where TValue : struct
        where TEvent : struct, IInteractionEvent
    {
        [SerializeField] private ValueTrigger<TValue> trigger;
        
        protected override void HandleStream(Guid id, IObservable<ValueEvent<TValue, TEvent>> stream)
        {
            base.HandleStream(id, stream);
            stream.Take(1).Subscribe(e =>
            {
                trigger.Trigger(e.Value);
            });
        }
    }
}