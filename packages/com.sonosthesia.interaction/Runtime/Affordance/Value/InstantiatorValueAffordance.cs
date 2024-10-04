using System;
using System.Linq;
using Sonosthesia.Channel;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class InstantiatorValueAffordance<TValue, TEvent, TContainer> : ValueAffordance<TValue, TEvent, TContainer>
        where TValue : struct
        where TEvent : struct, IValueEvent<TValue>
        where TContainer : MonoBehaviour, IValueEventStreamContainer<TValue, TEvent>
    {
        [SerializeField] private ScriptablePool<GameObject> _pool;

        [SerializeField] private Transform _attach;

        protected override void HandleStream(Guid id, IObservable<TEvent> stream)
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
                    UpdateAfforfanceInstance(id, instance, valueStreamHandlers, valueEvent);    
                }
                else
                {
                    doneAffordanceSetup = true;
                    SetupAfforfanceInstance(id, instance, valueStreamHandlers, valueEvent);    
                }
                
            });
            
            IObservable<TValue> valueStream = stream.Select(e => e.GetValue());
            valueStreamHandlers.Select(handler => handler.HandleStream(valueStream))
                .Merge().Subscribe(_ => { }, exception => { }, () =>
                {
                    _pool.Pool.Release(instance);
                });
        }
        
        protected virtual void SetupAfforfanceInstance(Guid id, GameObject instance, IStreamHandler<TValue>[] handlers, TEvent valueEvent)
        {
            
        }
        
        protected virtual void UpdateAfforfanceInstance(Guid id, GameObject instance, IStreamHandler<TValue>[] handlers, TEvent valueEvent)
        {
            
        }
    }
}