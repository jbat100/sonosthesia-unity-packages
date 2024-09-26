using System;
using UnityEngine;
using UniRx;
using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.Pack;

namespace Sonosthesia.PackMIDI
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
            
            Connect(receiver.NoteOnObservable, note => Broadcast(note.UnpackNoteOn()));
            Connect(receiver.NoteOffObservable, note => Broadcast(note.UnpackNoteOff()));
            Connect(receiver.ControlObservable, control => Broadcast(control.Unpack()));
            Connect(receiver.PolyphonicAftertouchObservable, aftertouch => Broadcast(aftertouch.Unpack()));
            Connect(receiver.ChannelAftertouchObservable, aftertouch => Broadcast(aftertouch.Unpack()));
            Connect(receiver.PitchBendObservable, bend => Broadcast(bend.Unpack()));

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