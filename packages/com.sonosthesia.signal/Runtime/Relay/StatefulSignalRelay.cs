using System;
using UniRx;

namespace Sonosthesia.Signal
{
    public class StatefulSignalRelay<TValue> : SignalRelay<TValue> where TValue : struct
    {
        private readonly BehaviorSubject<TValue> _subject = new BehaviorSubject<TValue>(default);
        public override IObservable<TValue> Observable => _subject.AsObservable();

        public override void Broadcast(TValue value) => _subject.OnNext(value);
    }
}