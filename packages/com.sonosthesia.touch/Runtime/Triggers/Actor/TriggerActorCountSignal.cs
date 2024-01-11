using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Touch
{
    /// <summary>
    /// Monitors the number of active events for a given actor and emits count as float signal
    /// </summary>
    public class TriggerActorCountSignal : Signal<float>
    {
        [SerializeField] private BaseTriggerActor _triggerActor;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _triggerActor.SourceStreamNode.Values.ObserveCountChanged(true)
                .Subscribe(count =>Broadcast((float)count));
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}