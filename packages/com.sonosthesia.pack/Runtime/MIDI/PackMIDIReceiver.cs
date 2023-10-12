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
        
        private readonly Subject<PackedMIDIChannelAftertouch> _channelAftertouchSubject = new ();
        internal IObservable<PackedMIDIChannelAftertouch> ChannelAftertouchObservable 
            => _channelAftertouchSubject.AsObservable();
        
        private readonly Subject<PackedMIDIPitchBend> _pitchBendSubject = new ();
        internal IObservable<PackedMIDIPitchBend> PitchBendObservable 
            => _pitchBendSubject.AsObservable();
        
        private readonly Subject<PackedMIDIClock> _clockSubject = new ();
        internal IObservable<PackedMIDIClock> ClockObservable 
            => _clockSubject.AsObservable();

        private protected virtual IDisposable Setup(AddressedPackConnection connection)
        {
            CompositeDisposable subscriptions = new();

            void Connect<T>(string address, IObserver<T> observer, Func<T, bool> filter = null)
            {
                subscriptions.Add(connection.IncomingContentObservable<T>(address)
                    .Where(item => filter == null || filter(item))
                    .ObserveOnMainThread()
                    .Subscribe(observer));
            }
            
            Connect(PackMIDIAddress.NOTE, _noteOnSubject, note => note.Velocity > 0);
            Connect(PackMIDIAddress.NOTE, _noteOffSubject, note => note.Velocity <= 0);
            Connect(PackMIDIAddress.NOTE_ON, _noteOnSubject);
            Connect(PackMIDIAddress.NOTE_OFF, _noteOffSubject);
            Connect(PackMIDIAddress.CONTROL, _controlSubject);
            Connect(PackMIDIAddress.POLYPHONIC_AFTERTOUCH, _polyphonicAftertouchSubject);
            Connect(PackMIDIAddress.CHANNEL_AFTERTOUCH, _channelAftertouchSubject);
            Connect(PackMIDIAddress.PITCH_BEND, _pitchBendSubject);
            Connect(PackMIDIAddress.CLOCK, _clockSubject);
            
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