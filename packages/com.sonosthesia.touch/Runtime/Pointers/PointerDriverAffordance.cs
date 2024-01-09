﻿using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public interface IPointerDriverAffordanceController<TValue> : IObserver<PointerDriverSource<TValue>.ValueEvent> where TValue : struct
    {
        
    }
    
    public class PointerDriverAffordance<TValue> : MonoBehaviour where TValue : struct
    {
        [SerializeField] private PointerDriverSource<TValue> _source;

        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void OnEventCountChanged(int count)
        {
            
        }

        protected virtual IPointerDriverAffordanceController<TValue> MakeController() => null;
        
        private void HandleStream(IObservable<PointerDriverSource<TValue>.ValueEvent> stream)
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
            _subscriptions.Add(_source.ValueStreamObservable.Subscribe(stream =>
            {
                HandleStream(stream.TakeUntilDisable(this));    
            }));
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}