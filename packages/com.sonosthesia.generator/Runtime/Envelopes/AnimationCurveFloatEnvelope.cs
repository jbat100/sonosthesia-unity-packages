using UnityEngine;

namespace Sonosthesia.Generator
{
    public class AnimationCurveFloatEnvelope : ValueEnvelope<float>
    {
        [SerializeField] private AnimationCurve _animationCurve;

        [SerializeField] private float _timeScale = 1f;
        
        public override float Duration
        {
            get
            {
                if (_animationCurve.length == 0)
                {
                    return 0;
                }
                return _animationCurve[_animationCurve.length - 1].time * _timeScale;
            }
        }

        public override float InitialValue 
        {
            get
            {
                if (_animationCurve.length == 0)
                {
                    return 0;
                }
                return _animationCurve[0].value;
            }
        }
        
        public override float FinalValue 
        {
            get
            {
                if (_animationCurve.length == 0)
                {
                    return 0;
                }
                return _animationCurve[_animationCurve.length - 1].value;
            }
        }

        public override float Evaluate(float t)
        {
            return _animationCurve.Evaluate(t / _timeScale);
        }
    }
}