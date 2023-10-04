using UnityEngine;
using UniRx;
using Sonosthesia.AdaptiveMIDI;

namespace Sonosthesia.Pack
{
    public class PackMIDIInput : MIDIInput
    {
        [SerializeField] private PackMIDIReceiver _receiver;

        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            _subscriptions.Add(_receiver.NoteOnObservable.Select(note => note.Unpack()).Subscribe(BroadcastNoteOn));
            _subscriptions.Add(_receiver.NoteOffObservable.Select(note => note.Unpack()).Subscribe(BroadcastNoteOff));
            _subscriptions.Add(_receiver.ControlObservable.Select(control => control.Unpack()).Subscribe(BroadcastControl));
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}