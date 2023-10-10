using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class PackMPEReceiver : MonoBehaviour
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
        
        private readonly Subject<PackedMPEAftertouch> _aftertouchSubject = new ();
        internal IObservable<PackedMPEAftertouch> AftertouchObservable 
            => _aftertouchSubject.AsObservable();
        
        private readonly Subject<PackedMPEBend> _bendSubject = new ();
        internal IObservable<PackedMPEBend> BendObservable 
            => _bendSubject.AsObservable();
        
        private protected virtual IDisposable Setup(AddressedPackConnection connection)
        {
            CompositeDisposable subscriptions = new();
            
            subscriptions.Add(connection.IncomingContentObservable<PackedMIDINote>(PackMPEAddress.NOTE)
                .Where(note => note.Velocity > 0)
                .ObserveOnMainThread()
                .Subscribe(_noteOnSubject));
            
            subscriptions.Add(connection.IncomingContentObservable<PackedMIDINote>(PackMPEAddress.NOTE)
                .Where(note => note.Velocity <= 0)
                .ObserveOnMainThread()
                .Subscribe(_noteOffSubject));
            
            subscriptions.Add(connection.IncomingContentObservable<PackedMIDINote>(PackMPEAddress.NOTE_ON)
                .ObserveOnMainThread()
                .Subscribe(_noteOnSubject));
            
            subscriptions.Add(connection.IncomingContentObservable<PackedMIDINote>(PackMPEAddress.NOTE_OFF)
                .ObserveOnMainThread()
                .Subscribe(_noteOffSubject));
            
            subscriptions.Add(connection.IncomingContentObservable<PackedMIDIControl>(PackMPEAddress.CONTROL)
                .ObserveOnMainThread()
                .Subscribe(_controlSubject));
            
            subscriptions.Add(connection.IncomingContentObservable<PackedMPEAftertouch>(PackMPEAddress.AFTERTOUCH)
                .ObserveOnMainThread()
                .Subscribe(_aftertouchSubject));
            
            subscriptions.Add(connection.IncomingContentObservable<PackedMPEBend>(PackMPEAddress.BEND)
                .ObserveOnMainThread()
                .Subscribe(_bendSubject));

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