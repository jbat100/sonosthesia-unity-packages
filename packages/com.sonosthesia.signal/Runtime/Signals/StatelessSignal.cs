using System;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public class StatelessSignal<T> : Signal<T>, ILogSwitch where T : struct
    {
        private readonly Subject<T> _signalSubject = new ();
        public override IObservable<T> Observable => _distinct ? _signalSubject.DistinctUntilChanged() : _signalSubject.AsObservable();
         
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