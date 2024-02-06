using Sonosthesia.AdaptiveMIDI.Messages;
using UnityEngine;

namespace Sonosthesia.MIDI
{
    public readonly struct MIDINote
    {
        public readonly int Channel;
        public readonly int Note;
        public readonly int Velocity;
        public readonly int Pressure;

        public MIDINote(int channel, int note, int velocity, int pressure = 0)
        {
            Channel = Mathf.Clamp(channel, 0, 15);
            Note = Mathf.Clamp(note, 0, 127);
            Velocity = Mathf.Clamp(velocity, 0, 127);
            Pressure = Mathf.Clamp(pressure, 0, 127);
        }

        public MIDINote(MIDINoteOn note, int pressure = 0)
        {
            Channel = note.Channel;
            Note = note.Note;
            Velocity = note.Velocity;
            Pressure = Mathf.Clamp(pressure, 0, 127);
        }
        
        public MIDINote(MIDINoteOff note, int pressure = 0)
        {
            Channel = note.Channel;
            Note = note.Note;
            Velocity = note.Velocity;
            Pressure = Mathf.Clamp(pressure, 0, 127);;
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