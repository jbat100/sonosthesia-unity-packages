using System;
using UnityEngine;
using UniRx;

namespace Sonosthesia.Channel
{
    public class Channel<T> : ChannelBase where T : struct
    {
        [SerializeField] private bool _log;
        
        private readonly Subject<IObservable<T>> _streamSubject = new ();
        public IObservable<IObservable<T>> StreamObservable => _streamSubject.AsObservable();

        public readonly struct State
        {
            public readonly T Current;
            public readonly float Start;
            public readonly int Count;

            public State(T value)
            {
                Current = value;
                Start = Time.time;
                Count = 0;
            }

            private State(T value, float start, int count)
            {
                Current = value;
                Start = start;
                Count = count;
            }

            public State Push(T value)
            {
                return new State(value, Start, Count + 1);
            }
        }

        private readonly ReactiveDictionary<Guid, State> _states = new();
        public IReadOnlyReactiveDictionary<Guid, State> States => _states;

        private readonly CompositeDisposable _subscriptions = new ();

        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            int count = 0;
            _subscriptions.Add(StreamObservable.Subscribe(stream =>
            {
                int streamCount = count++;
                Guid identifier = Guid.NewGuid();
                State? state = null;
                _subscriptions.Add(stream.Subscribe(value =>
                    {
                        if (_log)
                        {
                            Debug.Log($"{this} stream {streamCount} emitted {value}");    
                        }

                        bool initial = !state.HasValue;
                        state = state?.Push(value) ?? new State(value);
                        _states[identifier] = state.Value;
                        if (initial)
                        {
                            Register(identifier);    
                        }
                    },
                    e =>
                    {
                        if (_log)
                        {
                            Debug.LogError($"{this} stream {streamCount} error {e.Message}");    
                        }
                        _states.Remove(identifier);
                        Unregister(identifier);
                    },
                    () =>
                    {
                        if (_log)
                        {
                            Debug.Log($"{this} stream {streamCount} completed");    
                        }
                        _states.Remove(identifier);
                        Unregister(identifier);
                    }));
            }));
        }

        protected virtual void OnDisable()
        {
            _subscriptions.Clear();
            foreach (Guid identifier in _states.Keys)
            {
                Unregister(identifier);
            }
            _states.Clear();
        }
        
        public void Pipe(IObservable<T> observable)
        {
            _streamSubject.OnNext(observable);
        }
    }
}