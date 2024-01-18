using System;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI.Messages
{
    // polyphonic aftertouch and channel aftertouch are different kinds of messages 
    // https://learn.sparkfun.com/tutorials/midi-tutorial/advanced-messages
    
    public readonly struct MIDIPolyphonicAftertouch
    {
        public readonly TimeSpan Timestamp;
        public readonly int Channel;
        public readonly int Note;
        public readonly int Value;
        
        public MIDIPolyphonicAftertouch(TimeSpan timestamp, int channel, int note, int value)
        {
            Timestamp = timestamp;
            Channel = Mathf.Clamp(channel, 0, 15);
            Note = Mathf.Clamp(note, 0, 127);
            Value = Mathf.Clamp(value, 0, 127);
        }
        
        public MIDIPolyphonicAftertouch(int channel, int note, int value)
        {
            Timestamp = MIDIUtils.TimestampNow;
            Channel = Mathf.Clamp(channel, 0, 15);
            Note = Mathf.Clamp(note, 0, 127);
            Value = Mathf.Clamp(value, 0, 127);
        }

        public override string ToString()
        {
            return $"{nameof(MIDIPolyphonicAftertouch)} <{nameof(Channel)} {Channel} {nameof(Note)} {Note} {nameof(Value)} {Value}>";
        }
    }
}