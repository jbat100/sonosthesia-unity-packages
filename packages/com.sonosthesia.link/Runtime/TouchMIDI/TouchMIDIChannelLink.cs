using System;
using UnityEngine;
using Sonosthesia.MIDI;
using Sonosthesia.Touch;

namespace Sonosthesia.Link
{
    public class TouchMIDIChannelLink : ChannelLink<TouchPayload, MIDINote>
    {
        [SerializeField] private Mapping<int> _channelMapping;
        
        [SerializeField] private Mapping<int> _noteMapping;
        
        [SerializeField] private Mapping<int> _velocityMapping;
        
        protected override MIDINote Map(TouchPayload payload, TouchPayload reference, float timeOffset)
        {
            throw new NotImplementedException();
        }
    }
}