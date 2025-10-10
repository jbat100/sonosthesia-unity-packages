using Sonosthesia.Ease;
using Sonosthesia.Envelope;
using Sonosthesia.Utils;

namespace Sonosthesia.Interaction
{
    public enum EnvelopeInteraction
    {
        Constant,
        Pulse,
        Contact
    }
    
    public enum EnvelopeFilter
    {
        None,
        OneEuro
    }
    
    public interface IInteractiveEnvelopeSettings<TEvent> where TEvent : IInteractionEvent
    {
        EnvelopeInteraction Interaction { get; }
        FloatExtractorSettings<TEvent> ConstantExtractor { get; }
        FloatExtractorSettings<TEvent> ValueScaleExtractor { get; }
        FloatExtractorSettings<TEvent> TimeScaleExtractor { get; }
        FloatExtractorSettings<TEvent> ReleaseExtractor { get; }
        EnvelopeSettings Envelope { get; }
        EnvelopeFilter Filter { get; }
        OneEuroFilterSettings OneEuroFilter { get; }
        EaseType ReleaseType { get; }
        bool Track { get; }
    }
}