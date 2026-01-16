using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Sonosthesia.Utils;

#if UNITY_EDITOR
using System.Linq;
#endif

namespace Sonosthesia.Channel
{
    public class ChannelDispatcher<T> : Dispatcher where T : struct
    {
        [SerializeField] private InterfaceReference<IChannel<T>> _source;

        [SerializeField] private List<InterfaceReference<IChannel<T>>> _destinations;

        protected override int DestinationCount => _destinations.Count;
        
        private IDisposable _subscription;

        protected override void OnEnable()
        {
            base.OnEnable();
            _subscription?.Dispose();
            _subscription = _source.Value?.Observable.Subscribe(pair =>
            {
                if (_destinations.Count == 0)
                {
                    return;
                }
                _destinations[StepIndex()].Value?.Push(pair);
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
            _destinations.AddRange(GetComponentsInChildren<IChannel<T>>()
                .Select(c => new InterfaceReference<IChannel<T>>(c)));
        }

        public override void DeleteAllDestinations()
        {
            _destinations.Clear();
        }
#endif
    }
}