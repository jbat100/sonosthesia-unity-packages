using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Pack
{
    // separating the receiver from MIDIInput allows multiple MIDIInputs (filtering on port or track)
    // without needing the deserialization to be 
    
    public class PackLiveMIDIReceiver : MonoBehaviour
    {
        [SerializeField] private PackEnvelopeHub envelopeHub;

        private IDisposable _subscription;
        
        private readonly Subject<PackedLiveMIDINote> _noteOnSubject = new ();
        internal IObservable<PackedLiveMIDINote> NoteOnObservable 
            => _noteOnSubject.AsObservable();
        
        private readonly Subject<PackedLiveMIDINote> _noteOffSubject = new ();
        internal IObservable<PackedLiveMIDINote> NoteOffObservable 
            => _noteOffSubject.AsObservable();
        
        private readonly Subject<PackedLiveMIDIControl> _controlSubject = new ();
        internal IObservable<PackedLiveMIDIControl> ControlObservable 
            => _controlSubject.AsObservable();
        
        private readonly Subject<PackedLiveMIDIPolyphonicAftertouch> _polyphonicAftertouchSubject = new ();
        internal IObservable<PackedLiveMIDIPolyphonicAftertouch> PolyphonicAftertouchObservable 
            => _polyphonicAftertouchSubject.AsObservable();
        
        private readonly Subject<PackedLiveMIDIChannelAftertouch> _channelAftertouchSubject = new ();
        internal IObservable<PackedLiveMIDIChannelAftertouch> ChannelAftertouchObservable 
            => _channelAftertouchSubject.AsObservable();
        
        private readonly Subject<PackedLiveMIDIPitchBend> _pitchBendSubject = new ();
        internal IObservable<PackedLiveMIDIPitchBend> PitchBendObservable 
            => _pitchBendSubject.AsObservable();
        
        private protected virtual IDisposable Setup(PackEnvelopeHub envelopeHub)
        {
            CompositeDisposable subscriptions = new();

            void Connect<T>(string address, IObserver<T> observer, Func<T, bool> filter = null)
            {
                subscriptions.Add(envelopeHub.IncomingContentObservable<T>(address)
                    .Where(item => filter == null || filter(item))
                    .ObserveOnMainThread()
                    .Subscribe(observer));
            }
            
            Connect(PackLiveMIDIAddress.NOTE, _noteOnSubject, note => note.Velocity > 0);
            Connect(PackLiveMIDIAddress.NOTE, _noteOffSubject, note => note.Velocity <= 0);
            Connect(PackLiveMIDIAddress.NOTE_ON, _noteOnSubject);
            Connect(PackLiveMIDIAddress.NOTE_OFF, _noteOffSubject);
            Connect(PackLiveMIDIAddress.CONTROL, _controlSubject);
            Connect(PackLiveMIDIAddress.POLYPHONIC_AFTERTOUCH, _polyphonicAftertouchSubject);
            Connect(PackLiveMIDIAddress.CHANNEL_AFTERTOUCH, _channelAftertouchSubject);
            Connect(PackLiveMIDIAddress.PITCH_BEND, _pitchBendSubject);
            
            return subscriptions;
        }
        
        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = Setup(envelopeHub);
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        } 
    }
}