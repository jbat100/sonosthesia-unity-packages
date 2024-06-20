using UnityEngine;

namespace Sonosthesia.Generator
{
    public class ADSREnvelope : FloatEnvelope
    {
        [SerializeField] private float _attack = 1f;
        [SerializeField] private float _decay = 0.5f;
        [SerializeField] private float _sustain = 0.5f;
        [SerializeField] private float _release = 1f;

        // needs to be fixed AOT, if you want variable hold look at TrackedTriggerables 
        [SerializeField] private float _hold = 1f;
        
        public override float Duration => _attack + _decay + _hold + _release;

        public override float InitialValue => 0f;
        
        public override float FinalValue => 0f;

        public override float Evaluate(float t)
        {
            const float attackStart = 0;
            float decayStart = _attack;
            float holdStart = _attack + _decay;
            float releaseStart = holdStart + _hold;
            float end = releaseStart + _release;
            
            // attack phase
            if (t >= attackStart && t < decayStart)
            {
                return t / _attack;
            }
            // decay phase
            if (t >= decayStart && t < holdStart)
            {
                return 1 - (((t - decayStart) / _decay) * (1 - _sustain));
            }
            // hold phase
            if (t >= holdStart && t < releaseStart)
            {
                return _sustain;
            }
            // release phase
            if (t >= releaseStart && t < end)
            {
                return (1 - (t - releaseStart) / _release) * _sustain;
            }
            return 0;
        }
    }
}