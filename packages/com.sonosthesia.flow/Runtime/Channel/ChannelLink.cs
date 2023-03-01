using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class ChannelLink<TSource, TTarget> : MonoBehaviour where TSource : struct where TTarget : struct
    {
        [SerializeField] private bool _log;
        
        [SerializeField] private Channel<TSource> _source;

        [SerializeField] private Channel<TTarget> _target;

        private IDisposable _subscription;

        protected T Map<T>(Mapper<TSource, T> mapper, ValueProvider<T> provider, T fallback, TSource source, TSource reference, float timeOffset) where T :struct
        {
            if (mapper)
            {
                return mapper.Map(source, reference, timeOffset);
            }

            if (provider)
            {
                return provider.Value;
            }

            return fallback;
        }

        protected void OnEnable()
        {
            _subscription?.Dispose();
            if (_source)
            {
                _subscription = _source.StreamObservable.Subscribe(stream =>
                {
                    float startTime = Time.time;
                    TSource? reference = null;
                    IObservable<TTarget> mapped = stream
                        .Do(source => reference ??= source)
                        .Select(source => Map(source, reference.Value, Time.time - startTime));
                    if (_log)
                    {
                        mapped = mapped.Do(v => Debug.Log($"{this} mapped to {v}"));
                    }
                    _target.Pipe(mapped);
                });
            }
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        protected abstract TTarget Map(TSource payload, TSource reference, float timeOffset);
    }
}