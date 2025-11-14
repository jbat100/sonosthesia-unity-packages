using Sonosthesia.Interaction;

namespace Sonosthesia.Instrument
{
    public interface IMIDIControlConfiguration<in TEvent>
    {
        int ReferenceValue { get; }
        
        IInteractiveEnvelopeSettings<TEvent> Control { get; }       
        
    }
    
    public class MIDIControlConfiguration
    {
        
    }
}