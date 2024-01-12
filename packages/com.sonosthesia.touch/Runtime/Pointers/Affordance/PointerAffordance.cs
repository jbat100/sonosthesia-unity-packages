using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public interface IPointerAffordanceController<TValue> : IObserver<PointerValueEvent<TValue>> where TValue : struct
    {
        
    }
    
    public class PointerAffordance<TValue> : MonoBehaviour where TValue : struct
    {
        [SerializeField] private PointerSource<TValue> _source;

        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void OnEventCountChanged(int count)
        {
            
        }

        protected virtual IPointerAffordanceController<TValue> MakeController() => null;
        
        private void HandleStream(IObservable<PointerValueEvent<TValue>> stream)
        {
            IPointerAffordanceController<TValue> controller = MakeController();
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