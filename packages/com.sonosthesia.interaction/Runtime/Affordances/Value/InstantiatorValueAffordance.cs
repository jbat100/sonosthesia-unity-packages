using System;
using System.Linq;
using Sonosthesia.Channel;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class InstantiatorValueAffordance<TValue, TEvent> : ValueAffordance<TValue, TEvent>
        where TValue : struct
        where TEvent : struct, IInteractionEvent
    {
        [SerializeField] private ScriptablePool<GameObject> _pool;

        [SerializeField] private Transform _attach;

        protected override void HandleStream(Guid id, IObservable<ValueEvent<TValue, TEvent>> stream)
        {
            base.HandleStream(id, stream);
            
            Debug.Log($"{this} {nameof(HandleStream)} {id}");

            GameObject instance = _pool.Pool.Get();
            instance.transform.SetParent(_attach ? _attach : transform, false);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            
            IStreamHandler<TValue>[] valueStreamHandlers = instance.GetComponentsInChildren<IStreamHandler<TValue>>();

            bool doneAffordanceSetup = false;
            stream.Subscribe(valueEvent =>
            {
                if (doneAffordanceSetup)
                {
                    UpdateAffordanceInstance(id, instance, valueStreamHandlers, valueEvent);    
                }
                else
                {
                    doneAffordanceSetup = true;
                    SetupAffordanceInstance(id, instance, valueStreamHandlers, valueEvent);    
                }
                
            });
            
            IObservable<TValue> valueStream = stream.Select(e => e.Value);
            valueStreamHandlers.Select(handler => handler.HandleStream(valueStream))
                .Merge().Subscribe(_ => { }, exception => { }, () =>
                {
                    _pool.Pool.Release(instance);
                });
        }
        
        protected virtual void SetupAffordanceInstance(Guid id, GameObject instance, IStreamHandler<TValue>[] handlers, ValueEvent<TValue, TEvent> valueEvent)
        {
            
        }
        
        protected virtual void UpdateAffordanceInstance(Guid id, GameObject instance, IStreamHandler<TValue>[] handlers, ValueEvent<TValue, TEvent> valueEvent)
        {
            
        }
    }
}