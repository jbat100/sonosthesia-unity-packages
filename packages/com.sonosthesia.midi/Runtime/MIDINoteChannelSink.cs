using System;
using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Flow;
using UniRx;
using UnityEngine;

namespace Sonosthesia.MIDI
{
    public class MIDINoteChannelSink : MonoBehaviour
    {
        [SerializeField] private Channel<MIDINote> _channel;

        [SerializeField] private MIDIOutput _output;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _channel.StreamObservable.Subscribe(stream =>
            {
                MIDINote? initial = null;
                MIDINote? previous = null;
                stream.Subscribe(note =>
                {
                    if (initial.HasValue)
                    {
                        if (initial.Value.Note != note.Note)
                        {
                            Debug.LogError("Unexpected note mismatch withing MIDI note stream");
                            return;
                        }
                        if (initial.Value.Velocity != note.Velocity)
                        {
                            Debug.LogError("Unexpected velocity mismatch withing MIDI note stream");
                            return;
                        }
                        _output.BroadcastAftertouch(new MIDIPolyphonicAftertouch(note));
                    }
                    else
                    {
                        initial = note;
                        _output.BroadcastNoteOn(note);
                    }
                    previous = note;
                }, () =>
                {
                    if (previous.HasValue)
                    {
                        _output.BroadcastNoteOff(previous.Value);   
                    }
                });
            });
        }
        
    }
}