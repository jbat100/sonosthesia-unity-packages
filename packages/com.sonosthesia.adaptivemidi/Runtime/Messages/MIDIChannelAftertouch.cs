using System;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI.Messages
{
    // polyphonic aftertouch and channel aftertouch are different kinds of messages 
    // https://learn.sparkfun.com/tutorials/midi-tutorial/advanced-messages
    
    public readonly struct MIDIChannelAftertouch
    {
        public readonly TimeSpan Timestamp;
        public readonly int Channel;
        public readonly int Value;
        
        public MIDIChannelAftertouch(int channel, int value)
        {
            Timestamp = MIDIUtils.TimestampNow;
            Channel = Mathf.Clamp(channel, 0, 15);
            Value = Mathf.Clamp(value, 0, 127);
        }
        
        public MIDIChannelAftertouch(TimeSpan timestamp, int channel, int value)
        {
            Timestamp = timestamp;
            Channel = Mathf.Clamp(channel, 0, 15);
            Value = Mathf.Clamp(value, 0, 127);
        }
        
        public override string ToString()
        {
            return $"{nameof(MIDIChannelAftertouch)} <{nameof(Channel)} {Channel} {nameof(Value)} {Value} {nameof(Timestamp)} {Timestamp.TotalSeconds}>";
        }
    }
}