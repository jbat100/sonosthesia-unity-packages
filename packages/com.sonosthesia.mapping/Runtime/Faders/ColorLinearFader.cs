using UnityEngine;

namespace Sonosthesia.Flow
{
    public class ColorLinearFader : LinearFader<Color>
    {
        protected override Color Lerp(Color start, Color end, float value)
        {
            return Color.Lerp(start, end, value);
        }
    }
}