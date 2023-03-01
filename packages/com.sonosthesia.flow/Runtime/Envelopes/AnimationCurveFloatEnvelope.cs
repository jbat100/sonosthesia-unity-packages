using UnityEngine;

namespace Sonosthesia.Flow
{
    public class AnimationCurveFloatEnvelope : FloatEnvelope
    {
        [SerializeField] private AnimationCurve _animationCurve;
        
        public override float Duration()
        {
            return _animationCurve[_animationCurve.length - 1].time;
        }

        public override float Evaluate(float t)
        {
            return _animationCurve.Evaluate(t);
        }
    }
}