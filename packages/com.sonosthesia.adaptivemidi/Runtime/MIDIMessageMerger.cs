using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI
{
    public class MIDIMessageMerger : MIDIMessageNode
    {
        [SerializeField] private List<MIDIMessageNode> _broadcasters;

        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            foreach (MIDIMessageNode broadcaster in _broadcasters)
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