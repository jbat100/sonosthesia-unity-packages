using UnityEngine;

namespace Sonosthesia.Utils
{
    public static class AnimationCurveExtensions
    {
        public static float Duration(this AnimationCurve animationCurve)
        {
            return animationCurve[animationCurve.length - 1].time;
        }
        
        public static float Last(this AnimationCurve animationCurve)
        {
            return animationCurve[animationCurve.length - 1].value;
        }
    }
}