using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Sonosthesia.Channel;

namespace Sonosthesia.Arpeggiator
{
    public abstract class Arpeggiator<T> : Channel<T> where T : struct
    {
        [SerializeField] private Channel<T> _source;
        
        private IDisposable _subscription;
        
        protected abstract void HandleStream(KeyValuePair<Guid, IObservable<T>> pair);
        
        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_source)
            {
                _subscription = _source.Observable.Subscribe(HandleStream);
            }
        }
        
        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}