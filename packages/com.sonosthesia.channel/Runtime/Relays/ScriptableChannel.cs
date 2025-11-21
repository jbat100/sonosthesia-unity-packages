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
        
        private ChannelImplementation<T> _implementation;
        private ChannelImplementation<T> Implementation => _implementation ??= new ChannelImplementation<T>(this);
        
        public override IReadOnlyReactiveCollection<Guid> Ids => Implementation.Ids;
        public IReadOnlyReactiveDictionary<Guid, T> Values => Implementation.Values;
        public IObservable<KeyValuePair<Guid, IObservable<T>>> Observable => Implementation.Observable;
        
        public void Push(KeyValuePair<Guid, IObservable<T>> pair) => _implementation.Push(pair);
    }
}