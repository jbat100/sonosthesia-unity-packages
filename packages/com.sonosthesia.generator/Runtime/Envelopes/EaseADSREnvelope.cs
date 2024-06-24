using UnityEngine;

namespace Sonosthesia.Generator
{
    public class EaseADSREnvelope : ValueEnvelope<float>
    {
        [SerializeField] private EaseEnvelopePhase _attack = EaseEnvelopePhase.Linear(1f);
        [SerializeField] private EaseEnvelopePhase _decay = EaseEnvelopePhase.Linear(0.5f);
        [SerializeField] private EaseEnvelopePhase _release = EaseEnvelopePhase.Linear(1f);

        // needs to be fixed AOT, if you want variable hold look at TrackedTriggerables 
        [SerializeField] private float _sustain = 0.5f;
        [SerializeField] private float _hold = 1f;
        
        public override float Duration => _attack.Duration + _decay.Duration + _hold + _release.Duration;

        public override float InitialValue => 0f;
        
        public override float FinalValue => 0f;

        public override float Evaluate(float t)
        {
            const float attackStart = 0;
            float decayStart = _attack.Duration;
            float holdStart = decayStart + _decay.Duration;
            float releaseStart = holdStart + _hold;
            float end = releaseStart + _release.Duration;
            
            // attack phase
            if (t >= attackStart && t < decayStart)
            {
                return _attack.Evaluate(t);
            }
            // decay phase
            if (t >= decayStart && t < holdStart)
            {
                return 1 - _decay.Evaluate(t - decayStart) * (1 - _sustain);
            }
            // hold phase
            if (t >= holdStart && t < releaseStart)
            {
                return _sustain;
            }
            // release phase
            if (t >= releaseStart && t < end)
            {
                return (1 - _release.Evaluate(t - releaseStart)) * _sustain;
            }
            return 0;
        }
    }
}