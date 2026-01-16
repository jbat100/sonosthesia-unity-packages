using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Sonosthesia.Channel;
using Sonosthesia.Utils;

namespace Sonosthesia.Arpeggiator
{
    public abstract class Arpeggiator<T> : Channel<T> where T : struct
    {
        [SerializeField] private InterfaceReference<IChannel<T>> _source;
        
        private IDisposable _subscription;
        
        protected abstract void HandleStream(KeyValuePair<Guid, IObservable<T>> pair);
        
        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _source.Value?.Observable.Subscribe(HandleStream);
        }
        
        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}