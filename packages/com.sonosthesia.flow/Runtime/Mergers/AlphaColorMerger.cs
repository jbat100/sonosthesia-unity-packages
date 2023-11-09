using UnityEngine;

namespace Sonosthesia.Flow
{
    public class AlphaColorMerger : Merger<float, Color, Color>
    {
        protected override Color Combine(float alpha, Color color)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}