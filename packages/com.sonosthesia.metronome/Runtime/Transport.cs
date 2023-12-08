namespace Sonosthesia.Metronome
{
    // TODO check live https://cycling74.com/forums/metro-for-syncing-to-live-transport-quantized-midi-note-output
    
    // Transport as described by Ableton Live 
    
    public readonly struct Transport
    {
        public readonly int Bars;
        public readonly int Beats;
        public readonly int Sixteenths;

        public Transport(int bars, int beats, int sixteenths)
        {
            Bars = bars;
            Beats = beats;
            Sixteenths = sixteenths;
        }
    }
}