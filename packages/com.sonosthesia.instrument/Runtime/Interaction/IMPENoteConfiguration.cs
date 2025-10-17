using Sonosthesia.Interaction;

namespace Sonosthesia.Instrument
{
    public interface IMPENoteConfiguration<in TEvent>
    {
        bool ApplyPressure { get; }
        
        bool ApplySlide { get; }
        
        bool ApplyBend { get; }
        
        IMIDIPitchExtractor<TEvent> Pitch { get; }
        
        IStaticExtractor<TEvent, float> Velocity { get; }
        
        IDynamicExtractor<TEvent, float> Pressure { get; }
        
        IDynamicExtractor<TEvent, float> Slide { get; }
        
        IDynamicExtractor<TEvent, float> Bend { get; }
    }
}