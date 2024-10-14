namespace Sonosthesia.Envelope
{
    // IEnvelope interface allows for custom envelopes to be generated on the fly by an envelope generator
    // rather than necessarily be linked to a Component or ScriptableObject with static characteristics. This allows
    // for modulating envelopes based on different parameters. For example an envelope which lerps between two 
    // different ADSRs values. 
    
    public interface IEnvelope
    {
        float Duration { get; }
        float InitialValue { get; }
        float FinalValue { get; }
        float Evaluate(float time);
    }
    
    public static class EnvelopeExtensions
    {
        public static float End(this IEnvelope envelope)
        {
            return envelope.Evaluate(envelope.Duration);
        }
    }
}