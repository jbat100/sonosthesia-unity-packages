namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public readonly struct MIDINote
    {
        public readonly int Channel;
        public readonly int Note;
        public readonly int Velocity;
        public readonly int Pressure; // set with aftertouch messages

        public MIDINote(int channel, int note, int velocity)
        {
            Channel = channel;
            Note = note;
            Velocity = velocity;
            Pressure = 0;
        }
        
        public MIDINote(int channel, int note, int velocity, int pressure)
        {
            Channel = channel;
            Note = note;
            Velocity = velocity;
            Pressure = pressure;
        }

        public override string ToString()
        {
            return $"{nameof(MIDINote)} <{nameof(Channel)} {Channel} {nameof(Note)} {Note} {nameof(Velocity)} {Velocity} {nameof(Pressure)} {Pressure}>";
        }
    }
}