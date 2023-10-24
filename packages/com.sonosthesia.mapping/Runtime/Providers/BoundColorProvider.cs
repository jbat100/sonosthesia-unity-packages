using System;
using UnityEngine;

namespace Sonosthesia.Mapping
{
    public class BoundColorProvider : BoundProvider<Color>
    {
        private enum Space
        {
            RGBA,
            HSVA
        }

        [SerializeField] private Space _space;
        
        protected override Color Randomize(Color lower, Color upper)
        {
            switch (_space)
            {
                case Space.RGBA:
                {
                    float r = UnityEngine.Random.Range(lower.r, upper.r);
                    float g = UnityEngine.Random.Range(lower.g, upper.g);
                    float b = UnityEngine.Random.Range(lower.b, upper.b);
                    float a = UnityEngine.Random.Range(lower.a, upper.a);
                    return new Color(r, g, b, a);
                }
                case Space.HSVA:
                {
                    Color.RGBToHSV(lower, out float lh, out float ls, out float lv);
                    Color.RGBToHSV(upper, out float uh, out float us, out float uv);
                    float h = UnityEngine.Random.Range(lh, uh);
                    float s = UnityEngine.Random.Range(ls, us);
                    float v = UnityEngine.Random.Range(lv, uv);
                    Color color = Color.HSVToRGB(h, s, v);
                    color.a = UnityEngine.Random.Range(lower.a, upper.a);
                    return color;
                }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}