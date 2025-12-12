using System;
using System.Collections.Generic;
using Sonosthesia.Utils;
using UnityEngine;
using UniRx;

namespace Sonosthesia.Channel
{
    public interface IChannel<T> : IChannel
    {
        IReadOnlyReactiveDictionary<Guid, T> Values { get; }
        
        IObservable<KeyValuePair<Guid, IObservable<T>>> Observable { get; }

        void Push(KeyValuePair<Guid, IObservable<T>> pair);
    }

    public static class ChannelExtension
    {
        public static void Push<T>(this IChannel<T> channel, Guid id, IObservable<T> stream)
        {
            channel.Push(new KeyValuePair<Guid, IObservable<T>>(id, stream));
        }
        
        public static IDisposable Pipe<T>(this IChannel<T> channel, IChannel<T> other)
        {
            return other.Observable.Subscribe(channel.Push);
        }
    }

    public class ChannelImplementation<T> : IDisposable
    {
        private readonly ReactiveCollection<Guid> _streamIds = new();
        public IReadOnlyReactiveCollection<Guid> Ids => _streamIds;
        
        private readonly ReactiveDictionary<Guid, T> _values = new();
        public IReadOnlyReactiveDictionary<Guid, T> Values => _values;
        
        private readonly Subject<KeyValuePair<Guid, IObservable<T>>> _subject = new ();
        public IObservable<KeyValuePair<Guid, IObservable<T>>> Observable => _subject.AsObservable();
        
        private ILogSwitch _logSwitch;

        public ChannelImplementation(ILogSwitch logSwitch)
        {
            _logSwitch = logSwitch;
        }
        
        public void Push(KeyValuePair<Guid, IObservable<T>> pair) 
        {
            Guid id = pair.Key;
            IObservable<T> stream = pair.Value;
            _logSwitch.LogWarning($"{this} new stream {id}");
            pair.Value.Subscribe(
                v =>
                {
                    _logSwitch.LogVerbose($"{this} new stream value {v}");
                    _values[id] = v;
                    Register(id);
                }, 
                error =>
                {
                    _values.Remove(id);
                    Unregister(id);
                    _logSwitch.LogWarning($"{this} error end stream {id}");
                },
                () =>
                {
                    _values.Remove(id);
                    Unregister(id);
                    _logSwitch.LogWarning($"{this} completion end stream {id}");
                });
            
            _subject.OnNext(new KeyValuePair<Guid, IObservable<T>>(id, stream));
        }
        
        private void Register(Guid identifier)
        {
            if (!_streamIds.Contains(identifier))
            {
                _streamIds.Add(identifier);   
            }
        }

        private void Unregister(Guid identifier)
        {
            _streamIds.Remove(identifier);
        }

        public void Dispose()
        {
            _streamIds?.Dispose();
            _values?.Dispose();
            _subject?.Dispose();
        }
    }
    
    public class Channel<T> : AbstractChannel, IChannel<T>, ILogSwitch where T : struct
    {
        [SerializeField] private bool _log;
        public bool Log => _log;
        
        private ChannelImplementation<T> _implementation;
        private ChannelImplementation<T> Implementation => _implementation ??= new ChannelImplementation<T>(this);
        
        public override IReadOnlyReactiveCollection<Guid> Ids => Implementation.Ids;
        public IReadOnlyReactiveDictionary<Guid, T> Values => Implementation.Values;
        public IObservable<KeyValuePair<Guid, IObservable<T>>> Observable => Implementation.Observable;
        
        public void Push(KeyValuePair<Guid, IObservable<T>> pair) => Implementation.Push(pair);

        protected void OnDestroy() => _implementation?.Dispose();
    }
}