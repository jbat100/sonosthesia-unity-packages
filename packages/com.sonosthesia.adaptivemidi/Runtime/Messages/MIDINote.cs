using System;

namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public readonly struct MIDINote
    {
        public readonly TimeSpan Timestamp;
        public readonly int Channel;
        public readonly int Note;
        public readonly int Velocity;
        public readonly int Pressure; // set with polyphonic aftertouch messages

        public MIDINote(TimeSpan timestamp, int channel, int note, int velocity, int pressure = 0)
        {
            Timestamp = timestamp;
            Channel = channel;
            Note = note;
            Velocity = velocity;
            Pressure = pressure;
        }
        
        public MIDINote(int channel, int note, int velocity, int pressure = 0)
        {
            Timestamp = MIDIUtils.TimestampNow;
            Channel = channel;
            Note = note;
            Velocity = velocity;
            Pressure = pressure;
        }
        
        public MIDINote WithPressure(int pressure)
        {
            return new MIDINote(Channel, Note, Velocity, pressure);
        }

        public override string ToString()
        {
            return $"{nameof(MIDINote)} <{nameof(Channel)} {Channel} {nameof(Note)} {Note} {nameof(Velocity)} {Velocity} {nameof(Pressure)} {Pressure}>";
        }
    }
}