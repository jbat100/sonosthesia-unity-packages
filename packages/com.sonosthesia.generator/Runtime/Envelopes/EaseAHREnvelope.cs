using UnityEngine;

namespace Sonosthesia.Generator
{
    public class EaseAHREnvelope : ValueEnvelope<float>
    {
        [SerializeField] private EaseEnvelopePhase _attack = EaseEnvelopePhase.Linear(1f);
        [SerializeField] private EaseEnvelopePhase _hold = EaseEnvelopePhase.Linear(1f);
        [SerializeField] private EaseEnvelopePhase _release = EaseEnvelopePhase.Linear(1f);

        public override float Duration => _attack.Duration + _hold.Duration + _release.Duration;

        public override float InitialValue => 0f;
        
        public override float FinalValue => 0f;

        public override float Evaluate(float t)
        {
            const float attackStart = 0;
            float holdStart = _attack.Duration;
            float releaseStart = _attack.Duration + _hold.Duration;
            float end = releaseStart + _release.Duration;
            
            // attack phase
            if (t >= attackStart && t < holdStart)
            {
                return _attack.Evaluate(t);
            }
            // hold phase
            if (t >= holdStart && t < releaseStart)
            {
                return 1f;
            }
            // release phase
            if (t >= releaseStart && t < end)
            {
                if (_release.Duration < 1e-3)
                {
                    return 0f;
                }
                return 1f - _release.Evaluate(t - releaseStart);
            }
            return 0f;
        }
    }
}