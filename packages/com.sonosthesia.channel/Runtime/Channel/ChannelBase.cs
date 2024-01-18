using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Channel
{
    // allows observers who do not need specific types but are just interested in stream counts / ids

    public class ChannelBase : MonoBehaviour
    {
        private readonly ReactiveCollection<Guid> _streamIds = new();
        public IReadOnlyReactiveCollection<Guid> StreamIds => _streamIds;

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