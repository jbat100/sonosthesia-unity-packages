using Sonosthesia.AdaptiveMIDI.Messages;

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
            Channel = channel;
            Note = note;
            Velocity = velocity;
            Pressure = pressure;
        }

        public MIDINote(MIDINoteOn note, int pressure = 0)
        {
            Channel = note.Channel;
            Note = note.Note;
            Velocity = note.Velocity;
            Pressure = pressure;
        }
        
        public MIDINote(MIDINoteOff note, int pressure = 0)
        {
            Channel = note.Channel;
            Note = note.Note;
            Velocity = note.Velocity;
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