using System;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public abstract class ScriptableSignal<T> : ScriptableObject, ILogSwitch, ISignal<T> where T : struct
    {
        private readonly Subject<T> _signalSubject = new ();
        public IObservable<T> Observable => _distinct ? _signalSubject.DistinctUntilChanged() : _signalSubject.AsObservable();
         
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