using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class ColorHueAdaptor : MapAdaptor<float, Color>
    {
        [SerializeField, ColorUsage(true, false)] private Color _color;
        
        protected override Color Map(float source)
        {
            return _color.HueOffset(source, false);
        }
    }
}