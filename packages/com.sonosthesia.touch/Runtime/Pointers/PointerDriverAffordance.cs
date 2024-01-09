using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public interface IPointerDriverAffordanceController<TValue> : IObserver<PointerDriverSource<TValue>.SourceEvent> where TValue : struct
    {
        
    }
    
    public abstract class PointerDriverAffordance<TValue> : MonoBehaviour where TValue : struct
    {
        [SerializeField] private PointerDriverSource<TValue> _source;

        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void OnEventCountChanged(int count)
        {
            
        }
        
        protected abstract IPointerDriverAffordanceController<TValue> MakeController();
        
        private void HandleStream(IObservable<PointerDriverSource<TValue>.SourceEvent> stream)
        {
            IPointerDriverAffordanceController<TValue> controller = MakeController();
            if (controller != null)
            {
                stream.Subscribe(controller);
            }
        }

        protected virtual void OnEnable()
        {
            _subscriptions.Add(_source.OngoingEvents.ObserveCountChanged().Subscribe(OnEventCountChanged));
            _subscriptions.Add(_source.StreamObservable.Subscribe(stream =>
            {
                HandleStream(stream.TakeUntilDisable(this));    
            }));
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}