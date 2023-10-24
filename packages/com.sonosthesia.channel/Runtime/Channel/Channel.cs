using System;
using UnityEngine;
using UniRx;

namespace Sonosthesia.Flow
{
    public class Channel<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private bool _log;
        
        private readonly Subject<IObservable<T>> _streamSubject = new ();
        public IObservable<IObservable<T>> StreamObservable => _streamSubject.AsObservable();
        
        private readonly CompositeDisposable _logSubscriptions = new ();

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
        }


        public void Pipe(IObservable<T> observable)
        {
            _streamSubject.OnNext(observable);
        }
    }
}