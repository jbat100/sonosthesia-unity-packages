using System;
using Sonosthesia.Flow;
using UniRx;
using UnityEngine;
using Sonosthesia.Utils;
using Sonosthesia.Generator;

namespace Sonosthesia.Channel
{
    /// <summary>
    /// Uses the first value of each channel stream to trigger a TriggerFloatSignal using selectors
    /// for value and time factors
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChannelTrigger<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private TriggerFloatSignal _triggerFloatSignal;

        [SerializeField] private Channel<T> _channel;

        [SerializeField] private Selector<T> _valueSelector;

        [SerializeField] private Selector<T> _timeSelector;
        
        private IDisposable _subscription;
    
        protected void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _channel.StreamObservable
                .SelectMany(stream => stream.First())
                .Subscribe(note =>
                {
                    _triggerFloatSignal.Trigger(_valueSelector.Select(note), _timeSelector.Select(note));
                });
        }

        protected void OnDisable() => _subscription?.Dispose();
    }
}


