using UnityEngine;

namespace Sonosthesia.Generator
{
    public class EnvelopeFloatOscillator : FloatOscillator
    {
        [SerializeField] private ValueEnvelope<float> _envelope;

        protected override float Duration => _envelope.Duration;

        protected override float EvaluateIteration(float time) => _envelope.Evaluate(time);
    }
}