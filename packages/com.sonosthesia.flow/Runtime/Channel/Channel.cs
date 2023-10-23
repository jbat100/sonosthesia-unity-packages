using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Sonosthesia.Flow
{
    public class Channel<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private bool _log;
        
        private readonly Subject<IObservable<T>> _streamSubject = new ();
        public IObservable<IObservable<T>> StreamObservable => _streamSubject.AsObservable();

        private readonly Dictionary<Guid, BehaviorSubject<T>> _ongoingSubjects = new ();

        private readonly CompositeDisposable _logSubscriptions = new ();

        protected virtual void Awake()
        {
            
        }
        
        protected virtual void OnEnable()
        {
            _logSubscriptions.Clear();
            if (_log)
            {
                int count = 0;
                _logSubscriptions.Add(StreamObservable.Subscribe(stream =>
                {
                    int localCount = count++;
                    _logSubscriptions.Add(stream.Subscribe(value => Debug.Log($"{this} stream {localCount} emitted {value}"),
                            e => Debug.LogError($"{this} stream {localCount} error {e.Message}"),
                            () => Debug.Log($"{this} stream {localCount} completed")));
                }));

            }
        }

        protected virtual void OnDisable()
        {
            _logSubscriptions.Clear();
            foreach (BehaviorSubject<T> ongoing in _ongoingSubjects.Values)
            {
                ongoing.OnCompleted();
            }
            _ongoingSubjects.Clear();
        }

        protected Guid BeginEvent(T value)
        {
            Guid id = Guid.NewGuid();
            BehaviorSubject<T> subject = new BehaviorSubject<T>(value);
            _ongoingSubjects[id] = subject;
            _streamSubject.OnNext(subject.AsObservable());
            if (_log)
            {
                Debug.Log($"{this} {nameof(BeginEvent)} {id} {value}");   
            }
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
            if (_log)
            {
                Debug.Log($"{this} {nameof(EndEvent)}");   
            }
            subject.OnNext(end(subject.Value));
            subject.OnCompleted();
            subject.Dispose();
            _ongoingSubjects.Remove(id);
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
            if (_log)
            {
                Debug.Log($"{this} {nameof(EndEvent)}");   
            }
            subject.OnNext(end);
            subject.OnCompleted();
            subject.Dispose();
            _ongoingSubjects.Remove(id);
        }
        
        protected void EndEvent(Guid id)
        {
            if (!_ongoingSubjects.TryGetValue(id, out BehaviorSubject<T> subject))
            {
                throw new Exception($"invalid id {id}");
            }
            if (_log)
            {
                Debug.Log($"{this} {nameof(EndEvent)}");   
            }
            subject.OnCompleted();
            subject.Dispose();
            _ongoingSubjects.Remove(id);
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

        public void Pipe(IObservable<T> observable)
        {
            _streamSubject.OnNext(observable);
        }
    }
}