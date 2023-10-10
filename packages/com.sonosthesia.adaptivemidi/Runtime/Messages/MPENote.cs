namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public readonly struct MPENote
    {
        public readonly int Note;
        public readonly int Velocity;
        public readonly int Slide;
        public readonly int Pressure;
        public readonly float Bend;
        
        public MPENote(int note, int velocity, int slide, int pressure, float bend)
        {
            Note = note;
            Velocity = velocity;
            Slide = slide;
            Pressure = pressure;
            Bend = bend;
        }
        
        public override string ToString()
        {
            return $"{nameof(MPENote)} <{nameof(Note)} {Note} {nameof(Velocity)} {Velocity} {nameof(Slide)} {Slide} " +
                   $"{nameof(Pressure)} {Pressure} {nameof(Bend)} {Bend}>";
        }
    }
}