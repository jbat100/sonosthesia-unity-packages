using System;
using Sonosthesia.Utils;

namespace Sonosthesia.Envelope
{
    // Using Builder suffix for Component as opposed to the Factory suffix for the 
    
    [Obsolete("Use Envelope Factories")]
    public abstract class EnvelopeBuilder : ObservableBehaviour
    {
        public abstract IEnvelope Build();
    }

    public static class EnvelopeExtensions
    {
        public static float End<T>(this IEnvelope envelopeBuilder) where T : struct
        {
            return envelopeBuilder.Evaluate(envelopeBuilder.Duration);
        }
    }
}