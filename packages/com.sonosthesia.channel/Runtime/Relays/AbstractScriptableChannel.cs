using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Channel
{
    public abstract class AbstractScriptableChannel : ScriptableObject, IChannel
    {
        public abstract IReadOnlyReactiveCollection<Guid> Ids { get; }
    }
}