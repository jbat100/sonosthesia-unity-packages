using System;
using UnityEngine;
using UniRx;

namespace Sonosthesia.Flow
{
    public abstract class ScheduledArpegiator<T> : Arpegiator<T> where T : struct
    {
        [SerializeField] private Scheduler _scheduler;
        
        [SerializeField] private Modulator<T> _modulator;
        
        [SerializeField] private ArpegiatorFollower<T> _follower;

        [SerializeField] private ArpegiatorTerminator<T> _terminator;
        
        // Designed to handle a single 'note' stream from the source channel
        // - scheduler determines when an arpegiated stream (note) should be created
        // - modulator determines how the each arpegiated stream differs and follows the original 
        private class StreamArpegiator : IDisposable
        {
            private readonly float _startTime;
            private Scheduler.ISession _session;
            private IDisposable _streamSubscription;
            private IDisposable _sessionSubscription;
            private Subject<IObservable<T>> _arpegiations;

            public IObservable<IObservable<T>> Arpegiations => _arpegiations;
            
            public StreamArpegiator(IObservable<T> stream, 
                Scheduler scheduler, 
                Modulator<T> modulator,
                ArpegiatorFollower<T> follower,
                ArpegiatorTerminator<T> terminator)
            {
                IConnectableObservable<T> connected = stream.Replay(1);
                _startTime = Time.time;
                _session = scheduler.Session();
                _sessionSubscription = _session.Stream.Subscribe(offset => Arpegiate(connected, offset, modulator, follower, terminator));
                _arpegiations = new Subject<IObservable<T>>();
                _streamSubscription = stream.Subscribe(_ => {}, Dispose);
                connected.Connect();
            }

            public void Dispose()
            {
                _session?.Dispose();
                _session = null;
                _streamSubscription?.Dispose();
                _streamSubscription = null;
                _sessionSubscription?.Dispose();
                _sessionSubscription = null;
                _arpegiations?.OnCompleted();
                _arpegiations?.Dispose();
                _arpegiations = null;
            }

            private void Arpegiate(IObservable<T> stream, float offset, Modulator<T> modulator, ArpegiatorFollower<T> follower, ArpegiatorTerminator<T> terminator)
            {
                T? original = null;
                T? modulated = null;
                
                // the subject used to control the arpegiated stream
                IObservable<T> arpegiated = stream.Select(current =>
                {
                    original ??= current;
                    modulated ??= modulator.Modulate(current, offset);
                    return follower.Follow(original.Value, current, modulated.Value);
                });
                
                // the termination can be a function of both
                IObservable<Unit> termination = terminator.Termination(stream, arpegiated, Time.time - _startTime);
                
                // we prolong arpegiated because it can outlive stream if so determined by termination
                _arpegiations.OnNext(arpegiated
                    .Concat(Observable.Never<T>())
                    .TakeUntil(termination.IgnoreElements().Concat(Observable.Return(Unit.Default))));
            }
        }

        protected override void HandleStream(IObservable<T> stream)
        {
            StreamArpegiator arpegiator = new StreamArpegiator(stream, _scheduler, _modulator, _follower, _terminator);
            arpegiator.Arpegiations.Subscribe(Pipe);
        }
    }
}