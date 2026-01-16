using System;
using UniRx;
using UnityEngine;
using Sonosthesia.Mapping;
using Sonosthesia.Channel;
using Sonosthesia.Utils;

namespace Sonosthesia.Link
{
    public abstract class ChannelLink<TSource, TTarget> : MonoBehaviour where TSource : struct where TTarget : struct
    {
        [SerializeField] private bool _log;
        
        [SerializeField] private InterfaceReference<IChannel<TSource>> _source;

        [SerializeField] private InterfaceReference<IChannel<TTarget>> _target;

        [Serializable]
        public class Mapping<T> where T : struct
        {
            [SerializeField] private LinkMapper<TSource, T> linkMapper;
            [SerializeField] private ValueProvider<T> _provider;
            [SerializeField] private T _fallback;
            
            public T Map(TSource source, TSource reference, float timeOffset) 
            {
                if (linkMapper)
                {
                    return linkMapper.Map(source, reference, timeOffset);
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
            if (_source && _target)
            {
                _subscription = _source.Value.Observable.Subscribe(pair =>
                {
                    float startTime = Time.time;
                    TSource? reference = null;
                    IObservable<TTarget> mapped = pair.Value
                        .Do(source => reference ??= source)
                        .Select(source => Map(source, reference.Value, Time.time - startTime));
                    if (_log)
                    {
                        mapped = mapped.Do(v => Debug.Log($"{this} mapped to {v}"));
                    }
                    _target.Value.Push(pair.Key, mapped);
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