using Sonosthesia.Signal;

namespace Sonosthesia.Metronome
{
    public readonly struct Beat
    {
        public readonly bool Playing;
        public readonly float Position; // 1/16 note beats
        public readonly float? BPM; // 1/16 note beats

        private Beat(bool playing, float position, float? bpm)
        {
            Playing = playing;
            Position = position;
            BPM = bpm;
        }

        // BPM for 1/4 note beats as generally meant by DAWs
        public float? QuaterBPM => BPM / 4f;

        public static Beat Stop()
        {
            return new Beat(false, 0, 0);
        }

        public static Beat Play(float position, float? bpm)
        {
            return new Beat(true, position, bpm);
        }

        public override string ToString()
        {
            float? bpm = QuaterBPM;
            return $"{nameof(Beat)} {(Playing ? "Playing" : "Stopped")} {Position} {(bpm.HasValue ? $"{nameof(QuaterBPM)} {bpm.Value}" : "")}";
            //return $"{nameof(Beat)} {Position}";
        }
    
    }
    
    public class BeatSignal : Signal<Beat>
    {
        // http://midi.teragonaudio.com/tech/midispec/clock.htm
        // http://midi.teragonaudio.com/tech/midispec/seq.htm
    }
}
