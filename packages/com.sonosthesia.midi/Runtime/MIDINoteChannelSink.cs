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
                bool started = false;
                stream.Subscribe(note =>
                {
                    if (!started)
                    {
                        started = true;
                        
                    }
                }, () =>
                {
                    
                });
            });
        }
        
    }
}