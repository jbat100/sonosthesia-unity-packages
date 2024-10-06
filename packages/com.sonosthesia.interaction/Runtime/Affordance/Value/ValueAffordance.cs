using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class ValueAffordance<TValue, TEvent, TContainer> : MonoBehaviour 
        where TValue : struct
        where TEvent : struct, IValueEvent<TValue>
        where TContainer : MonoBehaviour, IValueEventStreamContainer<TValue, TEvent>
    {
        [SerializeField] private TContainer _container;

        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void OnEventCountChanged(int count)
        {
            
        }

        protected virtual IObserver<TEvent> MakeController(Guid id) => null;
        
        private void LinkController(Guid id, IObservable<TEvent> stream)
        {
            IObserver<TEvent> controller = MakeController(id);
            if (controller != null)
            {
                stream.Subscribe(controller);
            }
        }

        protected virtual void HandleStream(Guid id, IObservable<TEvent> stream)
        {
            
        }

        protected virtual void OnEnable()
        {
            
            _subscriptions.Add(_container.StreamNode.Values.ObserveCountChanged().Subscribe(OnEventCountChanged));
            _subscriptions.Add(_container.StreamNode.StreamObservable.Subscribe(pair =>
            {
                IObservable<TEvent> stream = pair.Value.TakeUntilDisable(this);
                LinkController(pair.Key, stream);    
                HandleStream(pair.Key, stream);
            }));
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}