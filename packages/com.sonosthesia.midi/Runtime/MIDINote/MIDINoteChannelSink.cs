using System;
using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.Channel;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.MIDI
{
    public class MIDINoteChannelSink : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IChannel<MIDINote>> _channel;

        [SerializeField] private InterfaceReference<IMIDIMessageBroadcaster> _output;

        [SerializeField] private bool _aftertouch;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _channel.Value?.Observable.Subscribe(pair =>
            {
                MIDINote? initial = null;
                MIDINote? previous = null;
                pair.Value.Subscribe(note =>
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
                            _output.Value.Broadcast(note.GetPolyphonicAftertouch());   
                        }
                    }
                    else
                    {
                        initial = note;
                        _output.Value.Broadcast(note.GetMIDINoteOn());
                        if (_aftertouch)
                        {
                            _output.Value.Broadcast(note.GetPolyphonicAftertouch());   
                        }
                    }
                    previous = note;
                }, () =>
                {
                    if (previous.HasValue)
                    {
                        _output.Value.Broadcast(previous.Value.GetMIDINoteOff());   
                    }
                });
            });
        }

        protected virtual void OnDisable() => _subscription?.Dispose();

    }
}