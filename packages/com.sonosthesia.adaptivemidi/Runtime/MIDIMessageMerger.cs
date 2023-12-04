using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI
{
    public class MIDIMessageMerger : MIDIMessageBroadcaster
    {
        [SerializeField] private List<MIDIMessageBroadcaster> _broadcasters;

        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            foreach (MIDIMessageBroadcaster broadcaster in _broadcasters)
            {
                _subscriptions.Add(Pipe(broadcaster));
            }
        }

        protected virtual void OnDisable()
        {
            _subscriptions.Clear();
        }
    }
}