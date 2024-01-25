using System;
using Sonosthesia.Flow;
using Sonosthesia.Signal;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Channel
{
    public class SelectionStreamHandler<TValue> : StreamHandler<TValue> where TValue : struct
    {
        [SerializeField] private Signal<float> _signal;

        [SerializeField] private Selector<TValue> _selector;

        protected override IDisposable InternalHandleStream(IObservable<TValue> stream)
        {
            return stream.Subscribe(value => _signal.Broadcast(_selector.Select(value)));
        }
    }
}