using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Channel
{
    public abstract class ChannelStreamHandler<T> : MonoBehaviour, IChannelStreamHandler<T> where T : struct
    {
        private Subject<Unit> _subject;
        private IDisposable _subscription;

        public IObservable<Unit> HandleStream(IObservable<T> stream)
        {
            Complete();
            _subject = new Subject<Unit>();
            _subscription = InternalHandleStream(stream);
            return _subject.AsObservable();
        }

        protected abstract IDisposable InternalHandleStream(IObservable<T> stream);

        protected virtual void Complete()
        {
            if (_subscription != null)
            {
                _subscription?.Dispose();
                _subscription = null;
            }
            if (_subject != null)
            {
                _subject.OnCompleted();
                _subject.Dispose();
                _subject = null;
            }
        }
    }
}