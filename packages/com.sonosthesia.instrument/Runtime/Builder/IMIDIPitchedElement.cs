using Sonosthesia.MIDI;

namespace Sonosthesia.Instrument
{
    public interface IMIDIPitchedElement
    {
        int MIDINote { get; set; } 
    }

    public static class MIDIPitchedElementExtensions
    {
        public static MIDIPitch GetMIDIPitch(this IMIDIPitchedElement element) => (MIDIPitch)element.MIDINote;

        public static void SetMIDIPitch(this IMIDIPitchedElement element, MIDIPitch pitch) => element.MIDINote = (int) pitch;
    }
}