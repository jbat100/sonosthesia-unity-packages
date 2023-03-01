using UnityEngine;

namespace Sonosthesia.Flow
{
    public class EnvelopeFloatOscillator : FloatOscillator
    {
        [SerializeField] private FloatEnvelope _envelope;

        protected override float Duration() => _envelope.Duration();

        protected override float EvaluateIteration(float time) => _envelope.Evaluate(time);
    }
}