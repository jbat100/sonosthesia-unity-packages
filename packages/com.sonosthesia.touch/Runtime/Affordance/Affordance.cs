using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class Affordance<TValue, TEvent, TSource> : MonoBehaviour 
        where TValue : struct
        where TEvent : struct, IEventValue<TValue>
        where TSource : MonoBehaviour, IValueStreamSource<TValue, TEvent>
    {
        [SerializeField] private TSource _source;

        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void OnEventCountChanged(int count)
        {
            
        }

        protected virtual IObserver<TEvent> MakeController() => null;
        
        private void HandleStream(IObservable<TEvent> stream)
        {
            IObserver<TEvent> controller = MakeController();
            if (controller != null)
            {
                stream.Subscribe(controller);
            }
        }

        protected virtual void OnEnable()
        {
            _subscriptions.Add(_source.ValueStreamNode.Values.ObserveCountChanged().Subscribe(OnEventCountChanged));
            _subscriptions.Add(_source.ValueStreamNode.StreamObservable.Subscribe(pair =>
            {
                HandleStream(pair.Value.TakeUntilDisable(this));    
            }));
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}