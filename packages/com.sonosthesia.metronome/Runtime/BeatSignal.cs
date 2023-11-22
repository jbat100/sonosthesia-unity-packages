using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Metronome
{
    public readonly struct Beat
    {
        public readonly bool Playing;
        public readonly float Position; // 16th notes

        public Beat(bool playing, float position)
        {
            Playing = playing;
            Position = position;
        }

    }
    
    public class BeatSignal : Signal<Beat>
    {
        // http://midi.teragonaudio.com/tech/midispec/clock.htm
        // http://midi.teragonaudio.com/tech/midispec/seq.htm
    }
}
