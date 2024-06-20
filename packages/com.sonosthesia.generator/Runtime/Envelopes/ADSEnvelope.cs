using UnityEngine;

namespace Sonosthesia.Generator
{
    /// <summary>
    /// Used in tandem with REnvelope for TrackedTriggerables
    /// </summary>
    public class ADSEnvelope : FloatEnvelope
    {
        [SerializeField] private float _attack;
        [SerializeField] private float _decay;
        [SerializeField] private float _sustain;
        
        public override float Duration => _attack + _decay;

        public override float InitialValue => 0f;
        
        public override float FinalValue => 0f;

        public override float Evaluate(float t)
        {
            const float attackStart = 0;
            float decayStart = _attack;
            float end = decayStart + _decay;
            
            // attack phase
            if (t >= attackStart && t < decayStart)
            {
                return t / _attack;
            }
            // decay phase
            if (t >= decayStart && t < end)
            {
                return 1 - (((t - decayStart) / _decay) * (1 - _sustain));
            }
            return 0;
        }
    }
}