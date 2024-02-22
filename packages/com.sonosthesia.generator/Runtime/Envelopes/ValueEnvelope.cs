namespace Sonosthesia.Generator
{
    public abstract class ValueEnvelope<T> : Generator<T> where T : struct
    {
        public abstract float Duration { get; }

        public abstract T InitialValue { get; }
        
        public abstract T FinalValue { get; }
    }

    public static class ValueEnvelopeExtensions
    {
        public static T End<T>(this ValueEnvelope<T> envelope) where T : struct
        {
            return envelope.Evaluate(envelope.Duration);
        }
    }
}