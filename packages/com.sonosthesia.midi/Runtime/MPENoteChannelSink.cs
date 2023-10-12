using System;
using System.Collections.Generic;
using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Flow;
using UniRx;
using UnityEngine;

namespace Sonosthesia.MIDI
{
    public class MPENoteChannelSink : MonoBehaviour
    {
        [SerializeField] private Channel<MPENote> _channel;
        
        [SerializeField] private MIDIOutput _output;

        private IDisposable _subscription;

        private readonly HashSet<int> _occupiedChannels = new();

        private bool TryGetAvailableChannel(out int channel)
        {
            for (int i = 2; i < 16; i++)
            {
                if (!_occupiedChannels.Contains(i))
                {
                    channel = i;
                    _occupiedChannels.Add(i);
                    return true;
                }
            }

            channel = -1;
            return false;
        }

        private bool ReleaseChannel(int channel)
        {
            return _occupiedChannels.Remove(channel);
        }

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _occupiedChannels.Clear();
            
            _subscription = _channel.StreamObservable.Subscribe(stream =>
            {
                if (!TryGetAvailableChannel(out int channel))
                {
                    Debug.LogWarning($"{this} failed to get free channel");
                    return;
                }

                MPENote? previous = null;
                stream.Subscribe(note =>
                {
                    if (previous.HasValue)
                    {
                        if (note.Slide != previous.Value.Slide)
                        {
                            _output.BroadcastControl(note.GetSlideControl(channel));
                        }
                        if (note.Pressure != previous.Value.Pressure)
                        {
                            _output.BrodcatstChannelAftertouch(note.GetChannelAftertouch(channel));
                        }
                        if (Math.Abs(note.Bend - previous.Value.Bend) > 1e-4)
                        {
                            _output.BroadcastPitchBend(note.GetPitchBend(channel));
                        }
                    }
                    else
                    {
                        // send slide, pressure, bend before note
                        _output.BroadcastControl(note.GetSlideControl(channel));
                        _output.BrodcatstChannelAftertouch(note.GetChannelAftertouch(channel));
                        _output.BroadcastPitchBend(note.GetPitchBend(channel));
                        _output.BroadcastNoteOn(note.GetMIDINoteOn(channel));
                    }

                    previous = note;
                }, () =>
                {
                    if (previous.HasValue)
                    {
                        _output.BroadcastNoteOff(previous.Value.GetMIDINoteOff(channel));   
                    }
                    ReleaseChannel(channel);
                    
                });
            });
        }
        
        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _occupiedChannels.Clear();
        }
    }
}