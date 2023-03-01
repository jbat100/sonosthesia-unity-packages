using UnityEngine;

namespace Sonosthesia.Flow
{
    public class FloatAnimationCurveFader : Fader<float>
    {
        [SerializeField] private AnimationCurve _animationCurve;
        
        public override float Fade(float fade)
        {
            return _animationCurve.Evaluate(fade);
        }
    }
}