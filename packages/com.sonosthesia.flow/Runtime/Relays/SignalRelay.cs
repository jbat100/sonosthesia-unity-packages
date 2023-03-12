using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class SignalRelay<T> : ScriptableObject where T : struct
    {
        private readonly BehaviorSubject<T> _subject = new BehaviorSubject<T>(default);
        public IObservable<T> Observable => _subject.AsObservable();

        public void Broadcast(T value) => _subject.OnNext(value);
    }
}