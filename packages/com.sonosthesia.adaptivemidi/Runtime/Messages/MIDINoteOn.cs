using System;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public readonly struct MIDINoteOn
    {
        public readonly TimeSpan Timestamp;
        public readonly int Channel;
        public readonly int Note;
        public readonly int Velocity;

        public MIDINoteOn(TimeSpan timestamp, int channel, int note, int velocity)
        {
            Timestamp = timestamp;
            Channel = Mathf.Clamp(channel, 0, 15);
            Note = Mathf.Clamp(note, 0, 127);
            Velocity = Mathf.Clamp(velocity, 0, 127);
        }
        
        public MIDINoteOn(int channel, int note, int velocity)
        {
            Timestamp = MIDIUtils.TimestampNow;
            Channel = Mathf.Clamp(channel, 0, 15);
            Note = Mathf.Clamp(note, 0, 127);
            Velocity = Mathf.Clamp(velocity, 0, 127);
        }

        public override string ToString()
        {
            return $"{nameof(MIDINoteOn)} <{nameof(Channel)} {Channel} {nameof(Note)} {Note} {nameof(Velocity)} {Velocity}>";
        }
    }
}