using System;

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
            Channel = channel;
            Note = note;
            Value = value;
        }
        
        public MIDIPolyphonicAftertouch(int channel, int note, int value)
        {
            Timestamp = MIDIUtils.TimestampNow;
            Channel = channel;
            Note = note;
            Value = value;
        }

        public MIDIPolyphonicAftertouch(MIDINote note)
        {
            Timestamp = MIDIUtils.TimestampNow;
            Channel = note.Channel;
            Note = note.Note;
            Value = note.Pressure;
        }

        public override string ToString()
        {
            return $"{nameof(MIDIPolyphonicAftertouch)} <{nameof(Channel)} {Channel} {nameof(Note)} {Note} {nameof(Value)} {Value}>";
        }
    }
}