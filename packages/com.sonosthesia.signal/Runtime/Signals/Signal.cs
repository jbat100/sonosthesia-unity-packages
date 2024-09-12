using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public class Signal<T> : MonoBehaviour where T : struct
    {
        private readonly BehaviorSubject<T> _signalSubject = new (default);
        public IObservable<T> SignalObservable => _distinct ? 
            _signalSubject.DistinctUntilChanged() : _signalSubject.AsObservable();

        public T Value => _signalSubject.Value;
         
        [SerializeField] private bool _log;

        [SerializeField] private bool _distinct;
        
        public void Broadcast(T value)
        {
            if (_log)
            {
                Debug.Log($"{this} {nameof(Broadcast)} {value}");    
            }
            OnBroadcast(value);
            _signalSubject.OnNext(value);
        }

        protected void OnDestroy()
        {
            _signalSubject.OnCompleted();
            _signalSubject.Dispose();
        }
        
        // hook for subclasses to react with predictable order (which is not the case for signal subscription)
        protected virtual void OnBroadcast(T value) { }
    }
}