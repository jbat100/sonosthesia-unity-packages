using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Palette.Flow
{
#if SONOSTHESIA_FLOW_SUPPORT
    using Sonosthesia.Flow;
    public class PaletteScannerColorAdaptor : MapAdaptor<float, Color>
    {
        [SerializeField] private float _value;
        
        [SerializeField] private List<Palette> _palettes;

        protected override Color Map(float source)
        {
            switch (_palettes.Count)
            { 
                case 0:
                    return Color.black;
                case 1:
                    return _palettes[0].Evaluate(_value);
                default:
                {
                    int floor = Mathf.FloorToInt(source);
                    
                    if (floor < 0)
                    {
                        return _palettes[0].Evaluate(_value);
                    }

                    if (floor >= _palettes.Count - 1)
                    {
                        return _palettes[^1].Evaluate(_value);
                    }

                    Color a = _palettes[floor].Evaluate(_value);
                    Color b = _palettes[floor+1].Evaluate(_value);
                    return Color.Lerp(a, b, source - floor);
                }
            }
        }
    }
#else 
    using System;
    // Should be warning rather than obsolete, just need the inspector help box
    [Obsolete("Requires com.sonosthesia.flow")]
    public class PaletteColorAdaptor : MonoBehaviour { }
#endif

}