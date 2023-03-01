using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class ValueEnvelope<T> : Generator<T> where T : struct
    {
        public abstract float Duration();
    }

    public static class ValueEnvelopeExtensions
    {
        public static T End<T>(this ValueEnvelope<T> envelope) where T : struct
        {
            return envelope.Evaluate(envelope.Duration());
        }
    }
}