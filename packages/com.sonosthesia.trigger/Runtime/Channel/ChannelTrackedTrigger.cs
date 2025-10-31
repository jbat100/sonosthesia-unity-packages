using System;
using UnityEngine;
using Sonosthesia.Channel;
using UniRx;

namespace Sonosthesia.Trigger
{
    [Obsolete("Use TriggerAffordance")]
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
            _subscription = _channel.Observable.TakeUntilDisable(this)
                .Subscribe(pair =>
                {
                    Guid id = pair.Key;
                    T? lastValue = null;

                    void EndTrigger()
                    {
                        if (!lastValue.HasValue)
                        {
                            return;
                        }
                        trigger.EndTrigger(_end, pair.Key, lastValue.Value);
                    }
                    
                    pair.Value.TakeUntilDisable(this).Subscribe(value =>
                    {
                        if (!lastValue.HasValue)
                        {
                            trigger.StartTrigger(pair.Key, _start, value);
                        }
                        lastValue = value;
                    }, error => EndTrigger(), EndTrigger);
                });
        }

        protected void OnDisable() => _subscription?.Dispose();
    }
}