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
        
        [SerializeField] private TrackedTrigger trigger;

        [SerializeField] private ValueStartTriggerSettings<T> _start;

        [SerializeField] private ValueEndTriggerSettings<T> _end;


        private IDisposable _subscription;
    
        protected void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _channel.StreamObservable
                .Subscribe(stream =>
                {
                    
                    Guid? id = null;
                    T? lastValue = null;

                    void EndTrigger()
                    {
                        if (!id.HasValue || !lastValue.HasValue)
                        {
                            return;
                        }
                        trigger.EndTrigger(_end, id.Value, lastValue.Value);
                    }
                    
                    stream.TakeUntilDisable(this).Subscribe(value =>
                    {
                        lastValue = value;
                        if (!id.HasValue)
                        {
                            id = trigger.StartTrigger(_start, value);
                        }
                    }, error =>
                    {
                        EndTrigger();
                    }, () =>
                    {
                        EndTrigger();
                    });
                });
        }

        protected void OnDisable() => _subscription?.Dispose();
    }
}