using System;
using System.Collections.Generic;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Channel
{
    public class ScriptableChannel<T> : AbstractScriptableChannel, IChannel<T>, ILogSwitch where T : struct
    {
        [SerializeField] private bool _log;
        public bool Log => _log;
        
        private readonly ReactiveDictionary<Guid, T> _values = new();
        public IReadOnlyReactiveDictionary<Guid, T> Values => _values;
        
        private readonly Subject<KeyValuePair<Guid, IObservable<T>>> _subject = new ();
        public IObservable<KeyValuePair<Guid, IObservable<T>>> Observable => _subject.AsObservable();
        
        public void Push(KeyValuePair<Guid, IObservable<T>> pair) 
        {
            Guid id = pair.Key;
            IObservable<T> stream = pair.Value;
            this.LogWarning($"{this} new stream {id}");
            pair.Value.Subscribe(
                v =>
                {
                    this.LogVerbose($"{this} new stream value {v}");
                    _values[id] = v;
                    Register(id);
                }, 
                error =>
                {
                    _values.Remove(id);
                    Unregister(id);
                    this.LogWarning($"{this} error end stream {id}");
                },
                () =>
                {
                    _values.Remove(id);
                    Unregister(id);
                    this.LogWarning($"{this} completion end stream {id}");
                });
            
            _subject.OnNext(new KeyValuePair<Guid, IObservable<T>>(id, stream));
        }
    }
}