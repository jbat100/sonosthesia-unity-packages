using System;
using Sonosthesia.Trigger;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class TriggerableValueAffordance<TValue, TEvent, TContainer> : ValueAffordance<TValue, TEvent, TContainer> 
        where TValue : struct
        where TEvent : struct, IValueEvent<TValue>
        where TContainer : MonoBehaviour, IValueEventStreamContainer<TValue, TEvent>
    {
        [SerializeField] private ValueTriggerable<TValue> _triggerable;
        
        protected override void HandleStream(Guid id, IObservable<TEvent> stream)
        {
            base.HandleStream(id, stream);
            stream.Take(1).Subscribe(e =>
            {
                _triggerable.Trigger(e.GetValue());
            });
        }
    }
}