namespace Sonosthesia.Instrument
{
    public class PitchedInstrumentElement : GroupTransformerElement, IMIDIPitchedElement
    {
        public int MIDINote { get; set; }
    }
}