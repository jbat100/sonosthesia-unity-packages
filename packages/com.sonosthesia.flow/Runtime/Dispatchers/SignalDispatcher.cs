using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using Sonosthesia.Signal;
using Sonosthesia.Utils;

namespace Sonosthesia.Flow
{
    public class SignalDispatcher<T> : Dispatcher where T : struct
    {
        [SerializeField] private InterfaceReference<ISignal<T>> _source;
        [SerializeField] private List<InterfaceReference<ISignal<T>>> _destinations = new ();
        
        private IDisposable _subscription;

        private void Setup()
        {
            _subscription?.Dispose();
            _subscription = _source.Value?.Observable.Subscribe(value =>
            {
                if (_destinations.Count == 0)
                {
                    return;
                }
                this.LogVerbose($"{this} dispatching {value}");
                _destinations[StepIndex()].Value?.Broadcast(value);
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
            _destinations.AddRange(GetComponentsInChildren<Signal<T>>()
                .Select(component => new InterfaceReference<ISignal<T>>(component)));
        }

        public override void DeleteAllDestinations()
        {
            _destinations.Clear();
        }
#endif
    }
}