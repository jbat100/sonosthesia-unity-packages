using System;
using UnityEngine;
using UniRx;
using Sonosthesia.AdaptiveMIDI;

namespace Sonosthesia.Pack
{
    public class PackLiveMIDIInput : MIDIInput
    {
        [SerializeField] private PackLiveMIDIReceiver _receiver;

        [SerializeField] private string _trackFilter;

        private IDisposable _subscription;

        protected virtual IDisposable Setup(PackLiveMIDIReceiver receiver)
        {
            CompositeDisposable subscriptions = new();

            void Connect<T>(IObservable<T> observable, Action<T> processor) where T : IPackedLiveMIDIMessage
            {
                subscriptions.Add(observable.Where(item => item.Check(_trackFilter)).Subscribe(processor));
            }
            
            Connect(receiver.NoteOnObservable, note => BroadcastNoteOn(note.Unpack()));
            Connect(receiver.NoteOffObservable, note => BroadcastNoteOff(note.Unpack()));
            Connect(receiver.ControlObservable, control => BroadcastControl(control.Unpack()));
            Connect(receiver.PolyphonicAftertouchObservable, aftertouch => BroadcastPolyphonicAftertouch(aftertouch.Unpack()));
            Connect(receiver.ChannelAftertouchObservable, aftertouch => BroadcastChannelAftertouch(aftertouch.Unpack()));
            Connect(receiver.PitchBendObservable, bend => BroadcastPitchBend(bend.Unpack()));

            return subscriptions;
        }
        
        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = Setup(_receiver);
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}