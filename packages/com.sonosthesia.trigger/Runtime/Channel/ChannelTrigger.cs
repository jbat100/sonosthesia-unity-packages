using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Utils;
using Sonosthesia.Channel;

namespace Sonosthesia.Trigger
{
    /// <summary>
    /// Uses the first value of each channel stream to trigger a TriggerFloatSignal using selectors
    /// for value and time factors
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChannelTrigger<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private Triggerable _triggerable;

        [SerializeField] private Channel<T> _channel;

        [SerializeField] private Selector<T> _valueSelector;

        [SerializeField] private Selector<T> _timeSelector;
        
        private IDisposable _subscription;
    
        protected void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _channel.StreamObservable
                .SelectMany(stream => stream.First())
                .Subscribe(value =>
                {
                    _triggerable.Trigger(_valueSelector.Select(value), _timeSelector.Select(value));
                });
        }

        protected void OnDisable() => _subscription?.Dispose();
    }
}


