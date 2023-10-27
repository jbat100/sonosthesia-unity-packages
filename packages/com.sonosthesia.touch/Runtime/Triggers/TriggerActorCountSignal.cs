using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Touch.Visuals
{
    public class TriggerActorCountSignal : Signal<float>
    {
        [SerializeField] private TriggerActorBase _triggerActor;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _triggerActor.States.ObserveCountChanged(true)
                .Subscribe(count =>Broadcast((float)count));
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}