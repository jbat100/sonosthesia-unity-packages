using System;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Channel
{
    public abstract class AbstractScriptableChannel : ScriptableObject, IGuidReactiveCollection
    {
        public abstract IReadOnlyReactiveCollection<Guid> Ids { get; }
    }
}