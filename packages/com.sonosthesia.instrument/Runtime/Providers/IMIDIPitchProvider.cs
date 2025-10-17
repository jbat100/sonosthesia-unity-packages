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
        
        public static bool TryExtract(this IMIDIPitchProvider provider, out int val)
        {
            if (provider != null)
            {
                val = provider.MIDIPitch;
                return true;
            }

            val = 0;
            return false;
        }
    }
}