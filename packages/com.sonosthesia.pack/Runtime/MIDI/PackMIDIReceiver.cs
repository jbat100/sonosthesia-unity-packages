using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Pack
{
    // separating the receiver from MIDIInput allows multiple MIDIInputs (filtering on port or track)
    // without needing the deserialization to be 
    
    public class PackMIDIReceiver : MonoBehaviour
    {
        [SerializeField] private AddressedPackConnection _connection;

        private IDisposable _subscription;
        
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
        
        private readonly Subject<PackedMIDIClock> _clockSubject = new ();
        internal IObservable<PackedMIDIClock> ClockObservable 
            => _clockSubject.AsObservable();

        private protected virtual IDisposable Setup(AddressedPackConnection connection)
        {
            CompositeDisposable subscriptions = new();
            
            subscriptions.Add(connection.IncomingContentObservable<PackedMIDINote>(PackMIDIAddress.NOTE)
                .Where(note => note.Velocity > 0)
                .ObserveOnMainThread()
                .Subscribe(_noteOnSubject));
            
            subscriptions.Add(connection.IncomingContentObservable<PackedMIDINote>(PackMIDIAddress.NOTE)
                .Where(note => note.Velocity <= 0)
                .ObserveOnMainThread()
                .Subscribe(_noteOffSubject));
            
            subscriptions.Add(connection.IncomingContentObservable<PackedMIDINote>(PackMIDIAddress.NOTE_ON)
                .ObserveOnMainThread()
                .Subscribe(_noteOnSubject));
            
            subscriptions.Add(connection.IncomingContentObservable<PackedMIDINote>(PackMIDIAddress.NOTE_OFF)
                .ObserveOnMainThread()
                .Subscribe(_noteOffSubject));
            
            subscriptions.Add(connection.IncomingContentObservable<PackedMIDIControl>(PackMIDIAddress.CONTROL)
                .ObserveOnMainThread()
                .Subscribe(_controlSubject));
            
            subscriptions.Add(connection.IncomingContentObservable<PackedMIDIPolyphonicAftertouch>(PackMIDIAddress.AFTERTOUCH)
                .ObserveOnMainThread()
                .Subscribe(_polyphonicAftertouchSubject));
            
            subscriptions.Add(connection.IncomingContentObservable<PackedMIDIClock>(PackMIDIAddress.CLOCK)
                .ObserveOnMainThread()
                .Subscribe(_clockSubject));

            return subscriptions;
        }
        
        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = Setup(_connection);
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        } 
    }
}