using UnityEngine;

namespace Sonosthesia.Generator
{
    /// <summary>
    /// Used in tandem with REnvelope for TrackedTriggerables
    /// </summary>
    public class ADSEnvelope : FloatEnvelope
    {
        [SerializeField] private float _attack = 1f;
        [SerializeField] private float _decay = 1f;
        [SerializeField] private float _sustain = 0.5f;
        
        public override float Duration => _attack + _decay;

        public override float InitialValue => 0f;
        
        public override float FinalValue => _sustain;

        public override float Evaluate(float t)
        {
            const float attackStart = 0;
            float decayStart = _attack;
            float end = decayStart + _decay;
            
            // attack phase
            if (t >= attackStart && t < decayStart)
            {
                if (_attack < 1e-3)
                {
                    return 1f;
                }
                return t / _attack;
            }
            // decay phase
            if (t >= decayStart && t < end)
            {
                if (_decay < 1e-3)
                {
                    return _sustain;
                }
                return 1 - (((t - decayStart) / _decay) * (1 - _sustain));
            }
            return _sustain;
        }
    }
}