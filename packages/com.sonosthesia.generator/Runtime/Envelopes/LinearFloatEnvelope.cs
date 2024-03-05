using UnityEngine;

namespace Sonosthesia.Generator
{
    public class LinearFloatEnvelope : FloatEnvelope
    {
        [SerializeField] private float _start = 0;
        
        [SerializeField] private float _end = 1;
        
        [SerializeField] private float _duration = 1;

        public override float Duration => _duration;

        public override float InitialValue => _start;

        public override float FinalValue => _end;

        public override float Evaluate(float t)
        {
            return Mathf.Lerp(_start, _end, t / _duration);
        }
    }
}