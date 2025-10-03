using Sonosthesia.Ease;
using Sonosthesia.Envelope;

namespace Sonosthesia.Interaction
{
    public enum InteractiveEnvelopeType
    {
        Constant,
        Pulse,
        Contact
    }
    
    public interface IInteractiveEnvelopeSettings<TEvent> where TEvent : IInteractionEvent
    {
        FloatExtractorSettings<TEvent> ConstantExtractor { get; }
        FloatExtractorSettings<TEvent> ValueScaleExtractor { get; }
        FloatExtractorSettings<TEvent> TimeScaleExtractor { get; }
        FloatExtractorSettings<TEvent> ReleaseExtractor { get; }
        EnvelopeSettings Envelope { get; }
        EaseType ReleaseType { get; }
        bool TrackValue { get; }
    }
}