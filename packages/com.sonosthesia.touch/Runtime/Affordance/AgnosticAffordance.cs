using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class AgnosticAffordance<TEvent, TSource> : MonoBehaviour 
        where TEvent : struct 
        where TSource : MonoBehaviour, IStreamSource<TEvent>
    {
        [SerializeField] private TSource _source;

        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void OnEventCountChanged(int count)
        {
            
        }

        protected virtual IObserver<TEvent> MakeController(Guid id) => null;
        
        private void HandleStream(Guid id, IObservable<TEvent> stream)
        {
            IObserver<TEvent> controller = MakeController(id);
            if (controller != null)
            {
                stream.Subscribe(controller);
            }
        }

        protected virtual void OnEnable()
        {
            _subscriptions.Add(_source.SourceStreamNode.Values.ObserveCountChanged().Subscribe(OnEventCountChanged));
            _subscriptions.Add(_source.SourceStreamNode.StreamObservable.Subscribe(pair =>
            {
                HandleStream(pair.Key, pair.Value.TakeUntilDisable(this));    
            }));
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}