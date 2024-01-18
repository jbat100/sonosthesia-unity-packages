using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Channel
{
    /// <summary>
    /// Merges streams from multiple source channels into a single target channel
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    
    public sealed class ChannelMerger<TValue> : MonoBehaviour where TValue : struct
    {
        [SerializeField] private Channel<TValue> _target;

        [SerializeField] private List<Channel<TValue>> _sources;

        private readonly Dictionary<Channel<TValue>, IDisposable> _subscriptions = new();

        private IDisposable Pipe(Channel<TValue> source, Channel<TValue> target)
        {
            return source.StreamObservable.Subscribe(target.Pipe);
        }

        private void OnEnable()
        {
            Clear();
            if (!_target)
            {
                return;
            }
            foreach (Channel<TValue> source in _sources)
            {
                if (!source)
                {
                    continue;
                }

                _subscriptions[source] = Pipe(source, _target);
            }
        }

        private void OnDisable()
        {
            Clear();
        }

        private void Clear()
        {
            foreach (IDisposable subscription in _subscriptions.Values)
            {
                subscription.Dispose();
            }
            _subscriptions.Clear();
        }

        public void Add(Channel<TValue> source)
        {
            if (_sources.Contains(source))
            {
                return;
            }
            _sources.Add(source);
            if (isActiveAndEnabled)
            {
                _subscriptions[source] = Pipe(source, _target);   
            }
        }

        public void Remove(Channel<TValue> source)
        {
            _sources.Remove(source);
            if (!_subscriptions.TryGetValue(source, out IDisposable subscription))
            {
                return;
            }
            subscription?.Dispose();
            _subscriptions.Remove(source);
        }

    }
}