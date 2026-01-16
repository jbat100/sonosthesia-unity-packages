using System;
using Sonosthesia.Channel;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Arpeggiator
{
    public abstract class Aftertoucher<T> : Channel<T> where T : struct
    {
        [SerializeField] private Channel<T> _source;

        [SerializeField] private Modulator<T> _modulator;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_source)
            {
                _subscription = _source.Observable.Subscribe(pair =>
                {
                    float startTime = Time.time;
                    // note : Rx Sample operator does not fire if first observable has not changed 
                    this.Push(pair.Key, pair.Value
                        .CombineLatest(UniRx.Observable.EveryUpdate(), (value, l) => _modulator.Modulate(value, Time.time - startTime))
                        .TakeUntil(pair.Value.IgnoreElements().AsUnitObservable().Concat(UniRx.Observable.Return(Unit.Default)))
                    );
                });
            }
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
        }
    }
}