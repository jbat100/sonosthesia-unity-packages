using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Mapping;
using Sonosthesia.Channel;

namespace Sonosthesia.Link
{
    // TODO move ChannelLink to a separate link package
    
    public abstract class ChannelLink<TSource, TTarget> : MonoBehaviour where TSource : struct where TTarget : struct
    {
        [SerializeField] private bool _log;
        
        [SerializeField] private Channel<TSource> _source;

        [SerializeField] private Channel<TTarget> _target;

        [Serializable]
        public class Mapping<T> where T : struct
        {
            [SerializeField] private Mapper<TSource, T> _mapper;
            [SerializeField] private ValueProvider<T> _provider;
            [SerializeField] private T _fallback;
            
            public T Map(TSource source, TSource reference, float timeOffset) 
            {
                if (_mapper)
                {
                    return _mapper.Map(source, reference, timeOffset);
                }
                if (_provider)
                {
                    return _provider.Value;
                }
                return _fallback;
            }
        }
        
        private IDisposable _subscription;

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