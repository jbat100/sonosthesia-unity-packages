using System;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class ValueAffordance<TValue, TEvent> : MonoBehaviour 
        where TValue : struct
        where TEvent : struct, IInteractionEvent
    {
        [SerializeField] private StreamContainer<ValueEvent<TValue, TEvent>> _container;

        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void OnEventCountChanged(int count)
        {
            
        }

        protected virtual IObserver<ValueEvent<TValue, TEvent>> MakeController(Guid id) => null;
        
        private void LinkController(Guid id, IObservable<ValueEvent<TValue, TEvent>> stream)
        {
            IObserver<ValueEvent<TValue, TEvent>> controller = MakeController(id);
            if (controller != null)
            {
                stream.Subscribe(controller);
            }
        }

        protected virtual void HandleStream(Guid id, IObservable<ValueEvent<TValue, TEvent>> stream)
        {
            
        }

        protected virtual void OnEnable()
        {
            
            _subscriptions.Add(_container.StreamNode.Values.ObserveCountChanged().Subscribe(OnEventCountChanged));
            _subscriptions.Add(_container.StreamNode.StreamObservable.Subscribe(pair =>
            {
                IObservable<ValueEvent<TValue, TEvent>> stream = pair.Value.TakeUntilDisable(this);
                LinkController(pair.Key, stream);    
                HandleStream(pair.Key, stream);
            }));
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}