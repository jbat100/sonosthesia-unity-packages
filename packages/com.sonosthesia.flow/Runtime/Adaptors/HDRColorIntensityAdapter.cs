using UnityEngine;

namespace Sonosthesia.Flow
{
    public class HDRColorIntensityAdapter : MapAdaptor<float, Color>
    {
        [SerializeField, ColorUsage(true, true)] private Color _color;
        
        protected override Color Map(float source)
        {
            float factor = Mathf.Pow(2f, source);
            return _color * factor;
        }
    }
}