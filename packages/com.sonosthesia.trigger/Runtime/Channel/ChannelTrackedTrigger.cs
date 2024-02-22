using System;
using UnityEngine;
using Sonosthesia.Utils;
using Sonosthesia.Channel;
using UniRx;

namespace Sonosthesia.Trigger
{
    public class ChannelTrackedTrigger<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private TrackedTriggerable _triggerable;

        [SerializeField] private Channel<T> _channel;

        [SerializeField] private Selector<T> _valueSelector;

        [SerializeField] private Selector<T> _timeSelector;
        
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
                        float timeScale = _timeSelector ? _timeSelector.Select(lastValue.Value) : 1f;
                        _triggerable.EndTrigger(id.Value, timeScale);
                    }
                    
                    stream.TakeUntilDisable(this).Subscribe(value =>
                    {
                        lastValue = value;
                        float valueScale = _valueSelector ? _valueSelector.Select(value) : 1f;
                        if (id.HasValue)
                        {
                            _triggerable.UpdateTrigger(id.Value, valueScale);
                        }
                        else
                        {
                            float timeScale = _timeSelector ? _timeSelector.Select(value) : 1f;
                            id = _triggerable.StartTrigger(valueScale, timeScale);
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