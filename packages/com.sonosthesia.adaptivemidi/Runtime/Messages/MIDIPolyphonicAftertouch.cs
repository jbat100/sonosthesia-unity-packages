namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public readonly struct MIDIPolyphonicAftertouch
    {
        public readonly int Channel;
        public readonly int Note;
        public readonly int Value;
        
        public MIDIPolyphonicAftertouch(int channel, int note, int value)
        {
            Channel = channel;
            Note = note;
            Value = value;
        }

        public override string ToString()
        {
            return $"{nameof(MIDIPolyphonicAftertouch)} <{nameof(Channel)} {Channel} {nameof(Note)} {Note} {nameof(Value)} {Value}>";
        }
    }
}