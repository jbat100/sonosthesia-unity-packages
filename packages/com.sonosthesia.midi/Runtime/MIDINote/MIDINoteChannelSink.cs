using System;
using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Extensions;
using Sonosthesia.Channel;
using UniRx;
using UnityEngine;

namespace Sonosthesia.MIDI
{
    public class MIDINoteChannelSink : MonoBehaviour
    {
        [SerializeField] private Channel<MIDINote> _channel;

        [SerializeField] private MIDIOutput _output;

        [SerializeField] private bool _aftertouch;

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
                        if (initial.Value.Channel != note.Channel)
                        {
                            Debug.LogError("Unexpected channel mismatch within MIDI note stream");
                            return;
                        }
                        if (initial.Value.Note != note.Note)
                        {
                            Debug.LogError("Unexpected note mismatch within MIDI note stream");
                            return;
                        }
                        if (initial.Value.Velocity != note.Velocity)
                        {
                            Debug.LogError("Unexpected velocity mismatch within MIDI note stream");
                            return;
                        }
                        if (_aftertouch && initial.Value.Pressure != note.Pressure)
                        {
                            _output.BroadcastPolyphonicAftertouch(note.Channel, note.Note, note.Pressure);   
                        }
                    }
                    else
                    {
                        initial = note;
                        _output.BroadcastNoteOn(note.Channel, note.Note, note.Velocity);
                        if (_aftertouch)
                        {
                            _output.BroadcastPolyphonicAftertouch(note.Channel, note.Note, note.Pressure);   
                        }
                    }
                    previous = note;
                }, () =>
                {
                    if (previous.HasValue)
                    {
                        _output.BroadcastNoteOff(previous.Value.Channel, previous.Value.Note, previous.Value.Velocity);   
                    }
                });
            });
        }

        protected virtual void OnDisable() => _subscription?.Dispose();

    }
}