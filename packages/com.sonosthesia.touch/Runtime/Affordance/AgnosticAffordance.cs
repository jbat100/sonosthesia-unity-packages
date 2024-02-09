using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    /// <summary>
    /// Affordance for a given event stream container, agnostic of value 
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TContainer"></typeparam>
    public class AgnosticAffordance<TEvent, TContainer> : MonoBehaviour 
        where TEvent : struct 
        where TContainer : MonoBehaviour, IEventStreamContainer<TEvent>
    {
        // TODO : relink in editor
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
            _subscriptions.Add(_container.EventStreamNode.Values.ObserveCountChanged().Subscribe(OnEventCountChanged));
            _subscriptions.Add(_container.EventStreamNode.StreamObservable.Subscribe(pair =>
            {
                IObservable<TEvent> stream = pair.Value.TakeUntilDisable(this); 
                LinkController(pair.Key, stream);
                HandleStream(pair.Key, stream);
            }));
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}