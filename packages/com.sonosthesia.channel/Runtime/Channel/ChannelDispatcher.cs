using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Sonosthesia.Utils;

namespace Sonosthesia.Channel
{
    public class ChannelDispatcher<T> : Dispatcher where T : struct
    {
        [SerializeField] private Channel<T> _source;

        [SerializeField] private List<Channel<T>> _destinations;

        protected override int DestinationCount => _destinations.Count;
        
        private IDisposable _subscription;

        protected override void OnEnable()
        {
            base.OnEnable();
            _subscription?.Dispose();
            _subscription = _source.StreamObservable.Subscribe(value =>
            {
                if (_destinations.Count == 0)
                {
                    return;
                }
                _destinations[StepIndex()].Pipe(value);
            });
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _subscription?.Dispose();
        }
        
#if UNITY_EDITOR
        public override void AutofillDestinations()
        {
            _destinations.AddRange(GetComponentsInChildren<Channel<T>>());
        }

        public override void DeleteAllDestinations()
        {
            _destinations.Clear();
        }
#endif
    }
}