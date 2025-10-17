namespace Sonosthesia.Instrument
{
    public interface IMIDIPitchExtractor<in TEvent>
    {
        bool TryExtractMIDIPitch(TEvent e, out int pitch);
    }
}