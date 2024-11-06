using System;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public class StatefulSignalRelay<TValue> : SignalRelay<TValue>, ILogSwitch where TValue : struct
    {
        [SerializeField] private bool _log;
        public bool Log => _log;
        
        private readonly BehaviorSubject<TValue> _subject = new (default);
        public override IObservable<TValue> Observable => _subject.AsObservable();
        public TValue Value => _subject.Value;

        public override void Broadcast(TValue value)
        {
            this.LogVerbose($"{this} {nameof(Broadcast)} {value}");
            _subject.OnNext(value);
        }
    }
}