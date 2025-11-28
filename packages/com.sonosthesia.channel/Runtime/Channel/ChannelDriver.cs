using System;
using System.Collections.Generic;
using UniRx;

namespace Sonosthesia.Channel
{
    public class ChannelDriver<T> : IDisposable where T : struct
    {
        public ChannelDriver(IChannel<T> channel)
        {
            _channel = channel;
        }
        
        private readonly IChannel<T> _channel;
        private readonly Dictionary<Guid, BehaviorSubject<T>> _ongoingSubjects = new ();

        public void Dispose()
        {
            EndAllStreams();
        }
        
        public Guid BeginStream(T value)
        {
            Guid id = Guid.NewGuid();
            BeginStream(value, id);
            return id;
        }
        
        public void BeginStream(T value, Guid id)
        {
            BehaviorSubject<T> subject = new (value);
            _ongoingSubjects[id] = subject;
            _channel.Push(id, subject.AsObservable());
        }

        public void UpdateStream(Guid id, Func<T, T> update)
        {
            if (!_ongoingSubjects.TryGetValue(id, out BehaviorSubject<T> subject))
            {
                throw new Exception($"invalid id {id}");
            }
            subject.OnNext(update(subject.Value));
        }
        
        public void EndStream(Guid id, Func<T, T> end)
        {
            if (!_ongoingSubjects.Remove(id, out BehaviorSubject<T> subject))
            {
                throw new Exception($"invalid id {id}");
            }

            subject.OnNext(end(subject.Value));
            subject.OnCompleted();
            subject.Dispose();
        }
        
        public void UpdateStream(Guid id, T updated)
        {
            if (!_ongoingSubjects.TryGetValue(id, out BehaviorSubject<T> subject))
            {
                throw new Exception($"invalid id {id}");
            }
            subject.OnNext(updated);
        }
        
        public void EndStream(Guid id, T end)
        {
            if (!_ongoingSubjects.Remove(id, out BehaviorSubject<T> subject))
            {
                throw new Exception($"invalid id {id}");
            }

            subject.OnNext(end);
            subject.OnCompleted();
            subject.Dispose();
        }
        
        public void EndStream(Guid id)
        {
            if (!_ongoingSubjects.Remove(id, out BehaviorSubject<T> subject))
            {
                throw new Exception($"invalid id {id}");
            }

            subject.OnCompleted();
            subject.Dispose();
        }

        public void EndAllStreams()
        {
            foreach (BehaviorSubject<T> behaviorSubject in _ongoingSubjects.Values)
            {
                behaviorSubject.OnCompleted();
                behaviorSubject.Dispose();
            }
            _ongoingSubjects.Clear();
        }
    }
}