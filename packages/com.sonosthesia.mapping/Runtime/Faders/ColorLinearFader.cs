using UnityEngine;

namespace Sonosthesia.Mapping
{
    public class ColorLinearFader : LinearFader<Color>
    {
        protected override Color Lerp(Color start, Color end, float value)
        {
            return Color.Lerp(start, end, value);
        }
    }
}