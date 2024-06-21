using UnityEngine;

namespace Sonosthesia.Generator
{
    public class AHREnvelope : FloatEnvelope
    {
        [SerializeField] private float _attack;
        [SerializeField] private float _hold;
        [SerializeField] private float _release;

        public override float Duration => _attack + _hold + _release;

        public override float InitialValue => 0f;
        
        public override float FinalValue => 0f;

        public override float Evaluate(float t)
        {
            const float attackStart = 0;
            float holdStart = _attack;
            float releaseStart = _attack + _hold;
            float end = releaseStart + _release;
            
            // attack phase
            if (t >= attackStart && t < holdStart)
            {
                if (_attack < 1e-3)
                {
                    return 1f;
                }
                return t / _attack;
            }
            // hold phase
            if (t >= holdStart && t < releaseStart)
            {
                return 1f;
            }
            // release phase
            if (t >= releaseStart && t < end)
            {
                if (_release < 1e-3)
                {
                    return 0f;
                }
                return 1f - (t - releaseStart) / _release;
            }
            return 0f;
        }
    }
}