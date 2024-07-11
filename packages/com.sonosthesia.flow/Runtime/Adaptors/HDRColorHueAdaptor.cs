using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class HDRColorHueAdaptor : MapAdaptor<float, Color>
    {
        [SerializeField, ColorUsage(true, true)] private Color _color;
        
        protected override Color Map(float source)
        {
            return _color.HueOffset(source, true);
        }
    }
}