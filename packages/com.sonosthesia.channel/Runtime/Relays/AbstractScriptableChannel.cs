using System;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Channel
{
    public class AbstractScriptableChannel : ScriptableObject, IGuidReactiveCollection
    {
        private readonly ReactiveCollection<Guid> _streamIds = new();
        public IReadOnlyReactiveCollection<Guid> Ids => _streamIds;

        protected void Register(Guid identifier)
        {
            if (!_streamIds.Contains(identifier))
            {
                _streamIds.Add(identifier);   
            }
        }

        protected void Unregister(Guid identifier)
        {
            _streamIds.Remove(identifier);
        }
    }
}