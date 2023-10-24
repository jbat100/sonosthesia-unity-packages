using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class Arpegiator<T> : Channel<T> where T : struct
    {
        [SerializeField] private Channel<T> _source;
        
        private IDisposable _subscription;
        
        protected abstract void HandleStream(IObservable<T> stream);
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _subscription?.Dispose();
            if (_source)
            {
                _subscription = _source.StreamObservable.Subscribe(HandleStream);
            }
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}