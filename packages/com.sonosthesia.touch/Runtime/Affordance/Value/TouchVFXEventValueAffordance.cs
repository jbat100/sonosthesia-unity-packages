using System;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.VFX;

namespace Sonosthesia.Touch
{
    public class TouchVFXEventValueAffordance<TValue, TContainer> : 
        ValueAffordance<TValue, TouchEvent, TContainer>
        where TValue : struct
        where TContainer : MonoBehaviour, IStreamContainer<ValueEvent<TValue, TouchEvent>>
    {
        [SerializeField] private VisualEffect _visualEffect;
        
        [SerializeField] private string _eventName;

        protected virtual void ConfigureEventAttribute(VFXEventAttribute eventAttribute, ValueEvent<TValue, TouchEvent> valueEvent)
        {
            
        }
        
        protected override void HandleStream(Guid id, IObservable<ValueEvent<TValue, TouchEvent>> stream)
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