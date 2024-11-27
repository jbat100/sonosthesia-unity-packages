using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Channel
{
    public class ChannelRelay<T> : ScriptableObject where T : struct
    {
        private readonly Subject<KeyValuePair<Guid, IObservable<T>>> _subject = new();
        public IObservable<KeyValuePair<Guid, IObservable<T>>> StreamObservable => _subject.AsObservable();

        public void Push(KeyValuePair<Guid, IObservable<T>> pair) => _subject.OnNext(pair);
    }
}