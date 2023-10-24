using System;
using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Touch;
using UnityEngine;

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