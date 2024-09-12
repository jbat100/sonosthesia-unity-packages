using UnityEngine;

namespace Sonosthesia.Palette.Flow
{
#if SONOSTHESIA_FLOW_SUPPORT
    using Sonosthesia.Flow;
    public class PaletteColorAdaptor : MapAdaptor<float, Color>
    {
        [SerializeField] private Palette _palette;

        protected override Color Map(float source)
        {
            return _palette ? _palette.Evaluate(source) : Color.black;
        }
    }
#else 
    using System;
    // Should be warning rather than obsolete, just need the inspector help box
    [Obsolete("Requires com.sonosthesia.flow")]
    public class PaletteColorAdaptor : MonoBehaviour { }
#endif

}