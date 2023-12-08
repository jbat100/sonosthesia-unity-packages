namespace Sonosthesia.Metronome
{
    public readonly struct Sync
    {
        public readonly bool Playing;
        public readonly float Position; // 1/16 note beats
        public readonly float? BPM; // 1/16 note beats

        private Sync(bool playing, float position, float? bpm)
        {
            Playing = playing;
            Position = position;
            BPM = bpm;
        }

        // BPM for 1/4 note beats as generally meant by DAWs
        public float? QuaterBPM => BPM / 4f;

        public static Sync Stop()
        {
            return new Sync(false, 0, 0);
        }

        public static Sync Play(float position, float? bpm)
        {
            return new Sync(true, position, bpm);
        }

        public override string ToString()
        {
            float? bpm = QuaterBPM;
            return $"{nameof(Sync)} {(Playing ? "Playing" : "Stopped")} {Position} {(bpm.HasValue ? $"{nameof(QuaterBPM)} {bpm.Value}" : "")}";
            //return $"{nameof(Beat)} {Position}";
        }
    }
}