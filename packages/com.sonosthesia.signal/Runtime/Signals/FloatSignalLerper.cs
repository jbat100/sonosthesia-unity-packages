using UnityEngine;

namespace Sonosthesia.Signal
{
    public class FloatSignalLerper : SignalLerper<float>
    {
        protected override float Lerp(float current, float target, float lerp)
        {
            return Mathf.Lerp(current, target, lerp);
        }
    }
}