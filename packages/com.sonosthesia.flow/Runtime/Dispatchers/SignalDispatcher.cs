using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Sonosthesia.Signal;
using Sonosthesia.Utils;

namespace Sonosthesia.Flow
{
    public class SignalDispatcher<T> : Dispatcher where T : struct
    {
        [SerializeField] private Signal<T> _source;

        [SerializeField] private List<Signal<T>> _destinations = new List<Signal<T>>();
        
        private IDisposable _subscription;

        private void Setup()
        {
            _subscription?.Dispose();
            _subscription = _source.SignalObservable.Subscribe(value =>
            {
                if (_destinations.Count == 0)
                {
                    return;
                }
                _destinations[StepIndex()].Broadcast(value);
            });
        }

        protected virtual void OnValidate()
        {
            base.OnEnable();
            Setup();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            Setup();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _subscription?.Dispose();
        }

        protected override int DestinationCount => _destinations.Count;
        
#if UNITY_EDITOR
        public override void AutofillDestinations()
        {
            _destinations.AddRange(GetComponentsInChildren<Signal<T>>());
        }

        public override void DeleteAllDestinations()
        {
            _destinations.Clear();
        }
#endif
    }
}