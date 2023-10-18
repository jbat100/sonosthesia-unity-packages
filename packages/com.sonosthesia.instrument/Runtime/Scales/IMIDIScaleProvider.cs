namespace Sonosthesia.Instrument
{
    public interface IMIDIScaleProvider<in TDescriptor>
    {
        MIDIScale GetScale(TDescriptor descriptor);
    }
}