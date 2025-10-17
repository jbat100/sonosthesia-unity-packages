namespace Sonosthesia.Instrument
{
    public interface IMIDIChannelExtractor<in TEvent>
    {
        bool TryExtractMIDIChannel(TEvent e, out int pitch);
    }
}