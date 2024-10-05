using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Utils;
using Sonosthesia.Channel;
using Sonosthesia.Envelope;

namespace Sonosthesia.Trigger
{
    /// <summary>
    /// Uses the first value of each channel stream to trigger a TriggerFloatSignal using selectors
    /// for value and time factors
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChannelTrigger<T> : MonoBehaviour where T : struct
    {
        [Header("Source")]
        
        [SerializeField] private Channel<T> _channel;
        
        [Header("Trigger")]
        
        [SerializeField] private Trigger trigger;

        [SerializeField] private ValueTriggerSettings<T> _settings;

        private IDisposable _subscription;
    
        protected void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _channel.StreamObservable
                .SelectMany(stream => stream.First())
                .Subscribe(value =>
                {
                    trigger.Trigger(_settings, value);
                });
        }

        protected void OnDisable() => _subscription?.Dispose();
    }
}


