using Sonosthesia.Extractor;

namespace Sonosthesia.Instrument
{
    public interface IMIDINoteConfiguration<in TEvent>
    {
        bool ApplyPressure { get; }
        
        IMIDIChannelExtractor<TEvent> Channel { get; }
        
        IMIDIPitchExtractor<TEvent> Pitch { get; }
        
        IStaticExtractor<TEvent, float> Velocity { get; }
        
        IDynamicExtractor<TEvent, float> Pressure { get; }
    }
}