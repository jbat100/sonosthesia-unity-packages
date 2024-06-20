using UnityEngine;

namespace Sonosthesia.Generator
{
    /// <summary>
    /// Used in tandem with ADSEnvelope for TrackedTriggerables
    /// </summary>
    public class SREnvelope : FloatEnvelope
    {
        [SerializeField] private float _sustain = 1f;
        
        [SerializeField] private float _release = 1f;
        
        public override float Duration => _release;

        public override float InitialValue => _sustain;
        
        public override float FinalValue => 0f;

        public override float Evaluate(float t)
        {
            if (t < _release)
            {
                return (1f - t / _release) * _sustain;
            }
            return 0;
        }
    }
}