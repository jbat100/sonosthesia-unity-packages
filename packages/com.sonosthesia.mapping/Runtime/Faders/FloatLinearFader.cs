using UnityEngine;

namespace Sonosthesia.Mapping
{
    public class FloatLinearFader : LinearFader<float>
    {
        protected override float LerpUnclamped(float start, float end, float value)
        {
            return Mathf.LerpUnclamped(start, end, value);
        }
    }
}