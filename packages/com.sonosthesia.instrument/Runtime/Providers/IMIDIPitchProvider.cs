using Sonosthesia.MIDI;

namespace Sonosthesia.Instrument
{
    public interface IMIDIPitchProvider
    {
        int MIDIPitch { get; set; } 
    }

    public static class MIDIPitchedElementExtensions
    {
        public static MIDIPitch GetMIDIPitch(this IMIDIPitchProvider element) => (MIDIPitch)element.MIDIPitch;

        public static void SetMIDIPitch(this IMIDIPitchProvider element, MIDIPitch pitch) => element.MIDIPitch = (int) pitch;
    }
}