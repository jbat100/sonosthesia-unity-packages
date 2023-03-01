using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class Signal<T> : MonoBehaviour where T : struct
    {
        private readonly BehaviorSubject<T> _signalSubject = new (default);
        public IObservable<T> SignalObservable => _signalSubject.AsObservable();

        public T Value => _signalSubject.Value;
         
        [SerializeField] private bool _log;
        
        protected void Broadcast(T value)
        {
            if (_log)
            {
                Debug.Log($"{this} {nameof(Broadcast)} {value}");    
            }
            _signalSubject.OnNext(value);
        }

        protected void OnDestroy()
        {
            _signalSubject.OnCompleted();
            _signalSubject.Dispose();
        }
    }
}