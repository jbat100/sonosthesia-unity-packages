using System;
using UniRx;

namespace Sonosthesia.Signal
{
    public class StatelessSignalRelay<TValue> : SignalRelay<TValue> where TValue : struct
    {
        private readonly Subject<TValue> _subject = new();
        public override IObservable<TValue> Observable => _subject.AsObservable();

        public override void Broadcast(TValue value) => _subject.OnNext(value);
    }
}