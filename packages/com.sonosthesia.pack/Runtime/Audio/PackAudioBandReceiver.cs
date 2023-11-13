using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public abstract class PackAudioBandReceiver<T> : MonoBehaviour where T : IPackedAudioBands
    {
        [SerializeField] private AddressedPackConnection _connection;

        private readonly CompositeDisposable _subscriptions = new ();
        
        private readonly Subject<T> _subject = new ();
        internal IObservable<T> BandObservable => _subject.AsObservable();

        // used to avoid excessive string comparisons downstream
        private readonly Dictionary<string, Subject<T>> _trackSubjects = new();
        
        protected abstract string PackAddress { get; }
        
        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            
            _subscriptions.Add(_connection.IncomingContentObservable<T>(PackAddress)
                .ObserveOnMainThread()
                .Subscribe(_subject));
            
            _subscriptions.Add(BandObservable.Subscribe(bands =>
            {
                if (_trackSubjects.TryGetValue(bands.Track, out Subject<T> subject))
                {
                    subject.OnNext(bands);
                }
            }));
        }
        
        protected virtual void OnDisable() => _subscriptions.Clear();

        protected virtual void OnDestroy()
        {
            foreach (Subject<T> subject in _trackSubjects.Values)
            {
                subject.OnCompleted();
                subject.Dispose();
            }
            _trackSubjects.Clear();
        }

        internal IObservable<T> TrackBandObservable(string track)
        {
            if (_trackSubjects.TryGetValue(track, out Subject<T> subject))
            {
                return subject.AsObservable();
            }
            subject = new Subject<T>();
            _trackSubjects[track] = subject;
            return subject.AsObservable();
        }
    }
}