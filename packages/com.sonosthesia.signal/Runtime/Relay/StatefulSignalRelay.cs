using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public class StatefulSignalRelay<TValue> : SignalRelay<TValue> where TValue : struct
    {
        [SerializeField] private bool _log;
        
        private readonly BehaviorSubject<TValue> _subject = new BehaviorSubject<TValue>(default);
        public override IObservable<TValue> Observable => _subject.AsObservable();

        public override void Broadcast(TValue value)
        {
            if (_log)
            {
                Debug.Log($"{this} {nameof(Broadcast)} {value}");
            }
            _subject.OnNext(value);
        }
    }
}