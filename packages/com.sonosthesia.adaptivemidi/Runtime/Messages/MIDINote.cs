namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public readonly struct MIDINote
    {
        public readonly int Channel;
        public readonly int Note;
        public readonly int Velocity;

        public MIDINote(int channel, int note, int velocity)
        {
            Channel = channel;
            Note = note;
            Velocity = velocity;
        }

        public override string ToString()
        {
            return $"{nameof(MIDINote)} <{nameof(Channel)} {Channel} {nameof(Note)} {Note} {nameof(Velocity)} {Velocity}>";
        }
    }
}