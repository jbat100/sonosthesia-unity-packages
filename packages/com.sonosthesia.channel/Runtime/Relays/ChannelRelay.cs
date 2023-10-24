using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Channel
{
    public class ChannelRelay<T> : ScriptableObject where T : struct
    {
        private readonly Subject<IObservable<T>> _subject = new();
        public IObservable<IObservable<T>> StreamObservable => _subject.AsObservable();

        public void Pipe(IObservable<T> stream) => _subject.OnNext(stream);
    }
}