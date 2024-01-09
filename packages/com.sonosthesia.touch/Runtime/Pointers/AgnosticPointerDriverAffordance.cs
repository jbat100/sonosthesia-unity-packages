﻿using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public interface IAgnosticPointerDriverAffordanceController : IObserver<BasePointerDriverSource.SourceEvent>
    {
        
    }
    
    public class AgnosticPointerDriverAffordance : MonoBehaviour
    {
        [SerializeField] private BasePointerDriverSource _source;

        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void OnEventCountChanged(int count)
        {
            
        }

        protected virtual IAgnosticPointerDriverAffordanceController MakeController() => null;
        
        private void HandleStream(IObservable<BasePointerDriverSource.SourceEvent> stream)
        {
            IAgnosticPointerDriverAffordanceController controller = MakeController();
            if (controller != null)
            {
                stream.Subscribe(controller);
            }
        }

        protected virtual void OnEnable()
        {
            _subscriptions.Add(_source.OngoingEvents.ObserveCountChanged().Subscribe(OnEventCountChanged));
            _subscriptions.Add(_source.SourceStreamObservable.Subscribe(stream =>
            {
                HandleStream(stream.TakeUntilDisable(this));    
            }));
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}