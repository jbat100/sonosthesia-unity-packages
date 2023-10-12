using UnityEngine;
using UniRx;
using Sonosthesia.AdaptiveMIDI;

namespace Sonosthesia.Pack
{
    public class PackMIDIInput : MIDIInput
    {
        [SerializeField] private PackMIDIReceiver _receiver;

        [SerializeField] private string _portFilter;

        [SerializeField] private string _trackFilter;

        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void OnEnable()
        {
            _subscriptions.Clear();

            bool FilterPort(IPackedMIDIPortMessage message)
            {
                return (string.IsNullOrEmpty(_portFilter) || message.Port == _portFilter) &&
                       (string.IsNullOrEmpty(_trackFilter) || message.Track == _trackFilter);
            }
            
            _subscriptions.Add(_receiver.NoteOnObservable
                .Where(FilterPort)
                .Select(note => note.Unpack())
                .Subscribe(BroadcastNoteOn));
            
            _subscriptions.Add(_receiver.NoteOffObservable
                .Where(FilterPort)
                .Select(note => note.Unpack())
                .Subscribe(BroadcastNoteOff));
            
            _subscriptions.Add(_receiver.ControlObservable
                .Where(FilterPort)
                .Select(control => control.Unpack())
                .Subscribe(BroadcastControl));
            
            _subscriptions.Add(_receiver.PolyphonicAftertouchObservable
                .Where(FilterPort)
                .Select(aftertouch => aftertouch.Unpack())
                .Subscribe(BroadcastPolyphonicAftertouch));
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}