using System;
using UnityEngine;

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
            Channel = Mathf.Clamp(channel, 0, 15);
            Note = Mathf.Clamp(note, 0, 127);
            Velocity = Mathf.Clamp(velocity, 0, 127);
        }
        
        public MIDINoteOff(int channel, int note, int velocity)
        {
            Timestamp = MIDIUtils.TimestampNow;
            Channel = Mathf.Clamp(channel, 0, 15);
            Note = Mathf.Clamp(note, 0, 127);
            Velocity = Mathf.Clamp(velocity, 0, 127);
        }

        public override string ToString()
        {
            return $"{nameof(MIDINoteOff)} <{nameof(Channel)} {Channel} {nameof(Note)} {Note} {nameof(Velocity)} {Velocity}>";
        }
    }
}