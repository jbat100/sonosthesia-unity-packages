using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Pack
{
    // separating the receiver from MIDIInput allows multiple MIDIInputs (filtering on port)
    // without needing the deserialization to be 
    
    public class PackMIDIReceiver : MonoBehaviour
    {
        [SerializeField] private AddressedPackConnection _connection;

        private CompositeDisposable _subscriptions = new ();
        
        private readonly Subject<PackedMIDINote> _noteOnSubject = new ();
        public IObservable<PackedMIDINote> NoteOnObservable => _noteOnSubject.AsObservable();
        
        private readonly Subject<PackedMIDINote> _noteOffSubject = new ();
        public IObservable<PackedMIDINote> NoteOffObservable => _noteOffSubject.AsObservable();
        
        private readonly Subject<PackedMIDIControl> _controlSubject = new ();
        public IObservable<PackedMIDIControl> ControlObservable => _controlSubject.AsObservable();

        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            _subscriptions.Add(_connection.PublishContent<PackedMIDINote>(PackMIDIAddress.NOTE_ON).Subscribe(_noteOnSubject));
            _subscriptions.Add(_connection.PublishContent<PackedMIDINote>(PackMIDIAddress.NOTE_OFF).Subscribe(_noteOffSubject));
            _subscriptions.Add(_connection.PublishContent<PackedMIDIControl>(PackMIDIAddress.CONTROL).Subscribe(_controlSubject));
        }
        
        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}