using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Sonosthesia.Scheduler;

namespace Sonosthesia.Arpeggiator
{
    public abstract class ScheduledArpeggiator<T> : Arpeggiator<T> where T : struct
    {
        [SerializeField] private LoopingScheduler _scheduler;

        [SerializeField] private Modulator<T> _modulator;
        
        [SerializeField] private ArpegiatorFollower<T> _follower;

        [SerializeField] private ArpegiatorTerminator<T> _terminator;
        
        // Designed to handle a single 'note' stream from the source channel
        // - scheduler determines when an arpegiated stream (note) should be created
        // - modulator determines how the each arpegiated stream differs and follows the original 
        private class StreamArpegiator : IDisposable
        {
            private readonly float _startTime;
            private ISchedulerSession _session;
            private IDisposable _streamSubscription;
            private IDisposable _sessionSubscription;
            private Subject<IObservable<T>> _arpeggiations;

            public IObservable<IObservable<T>> Arpeggiations => _arpeggiations;
            
            public StreamArpegiator(IObservable<T> stream, 
                LoopingScheduler scheduler, 
                Modulator<T> modulator,
                ArpegiatorFollower<T> follower,
                ArpegiatorTerminator<T> terminator)
            {
                IConnectableObservable<T> connected = stream.Replay(1);
                _startTime = Time.time;
                _session = scheduler.CreateSession(1f, 0f);
                _sessionSubscription = _session.Stream.Subscribe(offset => Arpegiate(connected, offset.position, modulator, follower, terminator));
                _arpeggiations = new Subject<IObservable<T>>();
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
                _arpeggiations?.OnCompleted();
                _arpeggiations?.Dispose();
                _arpeggiations = null;
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
                _arpeggiations.OnNext(arpegiated
                    .Concat(UniRx.Observable.Never<T>())
                    .TakeUntil(termination.IgnoreElements().Concat(UniRx.Observable.Return(Unit.Default))));
            }
        }

        protected override void HandleStream(KeyValuePair<Guid, IObservable<T>> pair)
        {
            StreamArpegiator arpegiator = new StreamArpegiator(pair.Value, _scheduler, _modulator, _follower, _terminator);
            arpegiator.Arpeggiations.Subscribe(arpeggiated => Push(Guid.NewGuid(), arpeggiated));
        }
    }
}