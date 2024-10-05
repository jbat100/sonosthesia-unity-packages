using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Touch
{
    /// <summary>
    /// Monitors the number of active events for a given actor and emits count as float signal
    /// </summary>
    public class TriggerStreamCount : Signal<float>
    {
        [SerializeField] private TriggerStream _stream;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _stream.EventStreamNode.Values.ObserveCountChanged(true)
                .Subscribe(count => Broadcast((float)count));
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}