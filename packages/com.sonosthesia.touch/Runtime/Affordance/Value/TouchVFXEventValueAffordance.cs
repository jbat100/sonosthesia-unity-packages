using System;
using Sonosthesia.Interaction;
using UniRx;
using UnityEngine;
using UnityEngine.VFX;

namespace Sonosthesia.Touch
{
    public class TouchVFXEventValueAffordance<TValue, TContainer> : 
        ValueAffordance<TValue, TriggerValueEvent<TValue>, TContainer>
        where TValue : struct
        where TContainer : MonoBehaviour, IValueEventStreamContainer<TValue, TriggerValueEvent<TValue>>
    {
        [SerializeField] private VisualEffect _visualEffect;
        
        [SerializeField] private string _eventName;

        protected virtual void ConfigureEventAttribute(VFXEventAttribute eventAttribute, TriggerValueEvent<TValue> valueEvent)
        {
            
        }
        
        protected override void HandleStream(Guid id, IObservable<TriggerValueEvent<TValue>> stream)
        {
            base.HandleStream(id, stream);
            stream.Take(1).Subscribe(e =>
            {
                VFXEventAttribute eventAttribute = _visualEffect.CreateVFXEventAttribute();
                ConfigureEventAttribute(eventAttribute, e);
                _visualEffect.SendEvent(_eventName, eventAttribute);
            });
        }
    }
}