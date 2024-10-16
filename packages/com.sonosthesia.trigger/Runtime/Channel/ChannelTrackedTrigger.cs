using System;
using UnityEngine;
using Sonosthesia.Channel;
using UniRx;

namespace Sonosthesia.Trigger
{
    public class ChannelTrackedTrigger<T> : MonoBehaviour where T : struct
    {
        [Header("Source")]
        
        [SerializeField] private Channel<T> _channel;
        
        [Header("Trigger")]
        
        [SerializeField] private Trigger trigger;

        [SerializeField] private ValueStartTriggerSettings<T> _start;

        [SerializeField] private ValueEndTriggerSettings<T> _end;


        private IDisposable _subscription;
    
        protected void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _channel.StreamObservable.TakeUntilDisable(this)
                .Subscribe(stream =>
                {
                    
                    Guid id = Guid.NewGuid();
                    T? lastValue = null;

                    void EndTrigger()
                    {
                        if (!lastValue.HasValue)
                        {
                            return;
                        }
                        trigger.EndTrigger(_end, id, lastValue.Value);
                    }
                    
                    stream.TakeUntilDisable(this).Subscribe(value =>
                    {
                        if (!lastValue.HasValue)
                        {
                            trigger.StartTrigger(_start, value);
                        }
                        lastValue = value;
                    }, error => EndTrigger(), EndTrigger);
                });
        }

        protected void OnDisable() => _subscription?.Dispose();
    }
}