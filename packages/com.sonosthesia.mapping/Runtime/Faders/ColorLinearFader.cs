using UnityEngine;

namespace Sonosthesia.Mapping
{
    public class ColorLinearFader : LinearFader<Color>
    {
        protected override Color LerpUnclamped(Color start, Color end, float value)
        {
            return Color.LerpUnclamped(start, end, value);
        }
    }
}