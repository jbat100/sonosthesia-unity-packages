using System;
using System.Collections.Generic;
using Sonosthesia.Utils;
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
        [SerializeField] private InterfaceReference<IChannel<TValue>> _target;

        [SerializeField] private List<InterfaceReference<IChannel<TValue>>> _sources;

        private readonly Dictionary<IChannel<TValue>, IDisposable> _subscriptions = new();

        private static IDisposable Pipe(IChannel<TValue> source, IChannel<TValue> target)
        {
            return source.Observable.Subscribe(target.Push);
        }

        private void OnEnable()
        {
            Clear();
            if (!_target)
            {
                return;
            }
            foreach (InterfaceReference<IChannel<TValue>> source in _sources)
            {
                if (!source)
                {
                    continue;
                }
                _subscriptions[source.Value] = Pipe(source.Value, _target.Value);
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
    }
}