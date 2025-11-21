using System;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Signal
{
    
    
    public class Signal<T> : MonoBehaviour, ILogSwitch where T : struct
    {
        private readonly BehaviorSubject<T> _signalSubject = new (default);
        public IObservable<T> SignalObservable => _distinct ? 
            _signalSubject.DistinctUntilChanged() : _signalSubject.AsObservable();

        public T Value => _signalSubject.Value;
         
        [SerializeField] private bool _log;
        public bool Log => _log;

        [SerializeField] private bool _distinct;
        
        public void Broadcast(T value)
        {
            this.LogVerbose($"{this} {nameof(Broadcast)} {value}");
            OnBroadcast(value);
            _signalSubject.OnNext(value);
        }

        protected virtual void OnDestroy()
        {
            _signalSubject.OnCompleted();
            _signalSubject.Dispose();
        }
        
        // hook for subclasses to react with predictable order (which is not the case for signal subscription)
        protected virtual void OnBroadcast(T value) { }
    }
}