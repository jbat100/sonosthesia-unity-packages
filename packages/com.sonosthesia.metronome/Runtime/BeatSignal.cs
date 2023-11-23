using Sonosthesia.Signal;

namespace Sonosthesia.Metronome
{
    public readonly struct Beat
    {
        public readonly bool Playing;
        public readonly float Position; // 1/16 notes
        public readonly float? BPM; // 1/16 notes

        private Beat(bool playing, float position, float? bpm)
        {
            Playing = playing;
            Position = position;
            BPM = bpm;
        }

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
            return $"{nameof(Beat)} {(Playing ? "Playing" : "Stopped")} {Position} {(BPM.HasValue ? $"{nameof(BPM)} {BPM.Value}" : "")}";
            //return $"{nameof(Beat)} {Position}";
        }
    
    }
    
    public class BeatSignal : Signal<Beat>
    {
        // http://midi.teragonaudio.com/tech/midispec/clock.htm
        // http://midi.teragonaudio.com/tech/midispec/seq.htm
    }
}
