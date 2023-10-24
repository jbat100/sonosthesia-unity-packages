using UnityEngine;

namespace Sonosthesia.Mapping
{
    public class FloatLinearFader : LinearFader<float>
    {
        protected override float Lerp(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, value);
        }
    }
}