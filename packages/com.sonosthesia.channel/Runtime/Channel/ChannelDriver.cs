using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class ChannelDriver<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private Channel<T> _channel;
        
        private readonly Dictionary<Guid, BehaviorSubject<T>> _ongoingSubjects = new ();

        protected virtual void OnDisable()
        {
            foreach (BehaviorSubject<T> ongoing in _ongoingSubjects.Values)
            {
                ongoing.OnCompleted();
                ongoing.Dispose();
            }
            _ongoingSubjects.Clear();
        }
        
        protected Guid BeginEvent(T value)
        {
            Guid id = Guid.NewGuid();
            BehaviorSubject<T> subject = new BehaviorSubject<T>(value);
            _ongoingSubjects[id] = subject;
            _channel.Pipe(subject.AsObservable());
            return id;
        }

        protected void UpdateEvent(Guid id, Func<T, T> update)
        {
            if (!_ongoingSubjects.TryGetValue(id, out BehaviorSubject<T> subject))
            {
                throw new Exception($"invalid id {id}");
            }
            subject.OnNext(update(subject.Value));
        }
        
        protected void EndEvent(Guid id, Func<T, T> end)
        {
            if (!_ongoingSubjects.TryGetValue(id, out BehaviorSubject<T> subject))
            {
                throw new Exception($"invalid id {id}");
            }
            _ongoingSubjects.Remove(id);
            subject.OnNext(end(subject.Value));
            subject.OnCompleted();
            subject.Dispose();
        }
        
        protected void UpdateEvent(Guid id, T updated)
        {
            if (!_ongoingSubjects.TryGetValue(id, out BehaviorSubject<T> subject))
            {
                throw new Exception($"invalid id {id}");
            }
            subject.OnNext(updated);
        }
        
        protected void EndEvent(Guid id, T end)
        {
            if (!_ongoingSubjects.TryGetValue(id, out BehaviorSubject<T> subject))
            {
                throw new Exception($"invalid id {id}");
            }
            _ongoingSubjects.Remove(id);
            subject.OnNext(end);
            subject.OnCompleted();
            subject.Dispose();
        }
        
        protected void EndEvent(Guid id)
        {
            if (!_ongoingSubjects.TryGetValue(id, out BehaviorSubject<T> subject))
            {
                throw new Exception($"invalid id {id}");
            }
            _ongoingSubjects.Remove(id);
            subject.OnCompleted();
            subject.Dispose();
        }

        protected void EndAllEvents()
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