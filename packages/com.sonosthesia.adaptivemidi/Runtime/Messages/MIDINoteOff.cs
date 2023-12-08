using System;

namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public readonly struct MIDINoteOff
    {
        public readonly TimeSpan Timestamp;
        public readonly int Channel;
        public readonly int Note;
        public readonly int Velocity;

        public MIDINoteOff(TimeSpan timestamp, int channel, int note, int velocity)
        {
            Timestamp = timestamp;
            Channel = channel;
            Note = note;
            Velocity = velocity;
        }
        
        public MIDINoteOff(int channel, int note, int velocity)
        {
            Timestamp = MIDIUtils.TimestampNow;
            Channel = channel;
            Note = note;
            Velocity = velocity;
        }

        public override string ToString()
        {
            return $"{nameof(MIDINoteOff)} <{nameof(Channel)} {Channel} {nameof(Note)} {Note} {nameof(Velocity)} {Velocity}>";
        }
    }
}