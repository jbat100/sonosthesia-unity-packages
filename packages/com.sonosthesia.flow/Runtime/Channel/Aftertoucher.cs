using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class Aftertoucher<T> : Channel<T> where T : struct
    {
        [SerializeField] private Channel<T> _source;

        [SerializeField] private Modulator<T> _modulator;

        private IDisposable _subscription;

        protected override void OnEnable()
        {
            base.OnEnable();
            _subscription?.Dispose();
            if (_source)
            {
                _subscription = _source.StreamObservable.Subscribe(stream =>
                {
                    float startTime = Time.time;
                    // note : Rx Sample operator does not fire if first observable has not changed 
                    Pipe(stream
                        .CombineLatest(Observable.EveryUpdate(), (value, l) => _modulator.Modulate(value, Time.time - startTime))
                        .TakeUntil(stream.IgnoreElements().AsUnitObservable().Concat(Observable.Return(Unit.Default)))
                    );
                });
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _subscription?.Dispose();
        }
    }
}