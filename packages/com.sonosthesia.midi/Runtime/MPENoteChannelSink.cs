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

        [Header("Debug")] 
        
        [SerializeField] private bool _forceChannel0;
        
        [SerializeField] private bool _muteSlide;
        
        [SerializeField] private bool _mutePressure;
        
        [SerializeField] private bool _muteBend;
        

        private IDisposable _subscription;

        private readonly HashSet<int> _occupiedChannels = new();

        private bool TryGetAvailableChannel(out int channel)
        {
            for (int i = 1; i < 15; i++)
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

        private void ReleaseChannel(int channel)
        {
            _occupiedChannels.Remove(channel);
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

                if (_forceChannel0)
                {
                    channel = 0;
                }

                MPENote? previous = null;
                stream.Subscribe(note =>
                {
                    if (previous.HasValue)
                    {
                        if (!_muteSlide && note.Slide != previous.Value.Slide)
                        {
                            _output.BroadcastChannelControl(note.GetSlideControl(channel));
                        }
                        if (!_mutePressure && note.Pressure != previous.Value.Pressure)
                        {
                            _output.BrodcatstChannelAftertouch(note.GetChannelAftertouch(channel));
                        }
                        if (!_muteBend && Math.Abs(note.Bend - previous.Value.Bend) > 1e-4)
                        {
                            _output.BroadcastPitchBend(note.GetPitchBend(channel));
                        }
                    }
                    else
                    {
                        // send slide, pressure, bend before note
                        if (!_muteSlide)
                        {
                            _output.BroadcastChannelControl(note.GetSlideControl(channel));    
                        }
                        if (!_mutePressure)
                        {
                            _output.BrodcatstChannelAftertouch(note.GetChannelAftertouch(channel));    
                        }
                        if (!_muteBend)
                        {
                            _output.BroadcastPitchBend(note.GetPitchBend(channel));
                        }
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