using UnityEngine;

namespace Sonosthesia.Envelope
{
    public class AnimationCurveEnvelope : IEnvelope
    {
        private readonly AnimationCurve _animationCurve;

        public AnimationCurveEnvelope(AnimationCurve animationCurve)
        {
            _animationCurve = animationCurve;
        }
        
        public float Duration
        {
            get
            {
                if (_animationCurve.length == 0)
                {
                    return 0;
                }
                return _animationCurve[_animationCurve.length - 1].time;
            }
        }

        public float InitialValue 
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
    
        public float FinalValue 
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

        public float Evaluate(float t)
        {
            return _animationCurve.Evaluate(t);
        }
    }
}