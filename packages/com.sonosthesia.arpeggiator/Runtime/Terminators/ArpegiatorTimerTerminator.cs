using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class ArpegiatorTimerTerminator<T> : ArpegiatorTerminator<T> where T : struct
    {
        private enum Reference
        {
            ArpegiatedStart,
            OriginalEnd,
            OffsetOriginalEnd
        }

        [SerializeField] private Reference _reference;

        [SerializeField] private float _duration;

        public override IObservable<Unit> Termination(IObservable<T> original, IObservable<T> arpegiated, float offset)
        {
            return _reference switch
            {
                Reference.ArpegiatedStart => Observable.Timer(TimeSpan.FromSeconds(_duration)).AsUnitObservable()
                    .IgnoreElements(),
                Reference.OriginalEnd => original.AsUnitObservable()
                    .Concat(Observable.Timer(TimeSpan.FromSeconds(_duration)).AsUnitObservable())
                    .IgnoreElements(),
                Reference.OffsetOriginalEnd => original.AsUnitObservable()
                    .Concat(Observable.Timer(TimeSpan.FromSeconds(_duration + offset)).AsUnitObservable())
                    .IgnoreElements(),
                _ => throw new NotImplementedException()
            };
        }
    }
}