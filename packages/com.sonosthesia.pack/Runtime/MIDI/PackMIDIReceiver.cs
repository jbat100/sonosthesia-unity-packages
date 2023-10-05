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
        internal IObservable<PackedMIDINote> NoteOnObservable 
            => _noteOnSubject.AsObservable();
        
        private readonly Subject<PackedMIDINote> _noteOffSubject = new ();
        internal IObservable<PackedMIDINote> NoteOffObservable 
            => _noteOffSubject.AsObservable();
        
        private readonly Subject<PackedMIDIControl> _controlSubject = new ();
        internal IObservable<PackedMIDIControl> ControlObservable 
            => _controlSubject.AsObservable();
        
        private readonly Subject<PackedMIDIPolyphonicAftertouch> _polyphonicAftertouchSubject = new ();
        internal IObservable<PackedMIDIPolyphonicAftertouch> PolyphonicAftertouchObservable 
            => _polyphonicAftertouchSubject.AsObservable();

        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            
            _subscriptions.Add(_connection.IncomingContentObservable<PackedMIDINote>(PackMIDIAddress.NOTE)
                .Where(note => note.Velocity > 0)
                .ObserveOnMainThread()
                .Subscribe(_noteOnSubject));
            
            _subscriptions.Add(_connection.IncomingContentObservable<PackedMIDINote>(PackMIDIAddress.NOTE)
                .Where(note => note.Velocity <= 0)
                .ObserveOnMainThread()
                .Subscribe(_noteOffSubject));
            
            _subscriptions.Add(_connection.IncomingContentObservable<PackedMIDINote>(PackMIDIAddress.NOTE_ON)
                .ObserveOnMainThread()
                .Subscribe(_noteOnSubject));
            
            _subscriptions.Add(_connection.IncomingContentObservable<PackedMIDINote>(PackMIDIAddress.NOTE_OFF)
                .ObserveOnMainThread()
                .Subscribe(_noteOffSubject));
            
            _subscriptions.Add(_connection.IncomingContentObservable<PackedMIDIControl>(PackMIDIAddress.CONTROL)
                .ObserveOnMainThread()
                .Subscribe(_controlSubject));
            
            _subscriptions.Add(_connection.IncomingContentObservable<PackedMIDIPolyphonicAftertouch>(PackMIDIAddress.AFTERTOUCH)
                .ObserveOnMainThread()
                .Subscribe(_polyphonicAftertouchSubject));
        }
        
        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}