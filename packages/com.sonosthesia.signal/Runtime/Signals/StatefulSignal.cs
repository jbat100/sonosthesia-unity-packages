using System;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public interface IStatefulSignal<T> : ISignal<T> where T : struct
    {
        T Value { get; }
    }
    
    public class StatefulSignal<T> : Signal<T>, ILogSwitch, IStatefulSignal<T> where T : struct
    {
        private readonly BehaviorSubject<T> _signalSubject = new (default);
        public override IObservable<T> Observable => _distinct ? 
            _signalSubject.DistinctUntilChanged() : _signalSubject.AsObservable();

        public T Value => _signalSubject.Value;
         
        [SerializeField] private bool _log;
        public bool Log => _log;

        [SerializeField] private bool _distinct;
        
        public override void Broadcast(T value)
        {
            this.LogVerbose($"{this} {nameof(Broadcast)} {value}");
            _signalSubject.OnNext(value);
        }

        protected virtual void OnDestroy()
        {
            _signalSubject.OnCompleted();
            _signalSubject.Dispose();
        }
    }
}