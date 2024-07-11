using UnityEngine;

namespace Sonosthesia.Utils
{
    public struct HSVColor
    {
        public float h;
        public float s;
        public float v;
        public float a;

        public override string ToString()
        {
            return $"HSVA({h:F2} {s:F2} {v:F2} {a:F2})";
        }
    }

    public static class HSVColorExtensions
    {
        public static Color HueOffset(this Color c, float offset, bool hdr = false)
        {
            HSVColor hsv = c.ToHSV();
            hsv.h = (hsv.h + offset) % 1f;
            return hsv.ToRGB(hdr);
        }
        
        public static HSVColor ToHSV(this Color rgb)
        {
            Color.RGBToHSV(rgb, out float H, out float S, out float V);
            return new HSVColor
            {
                h = H,
                s = S,
                v = V,
                a = rgb.a
            };
        }

        public static Color ToRGB(this HSVColor hsv, bool hdr = false)
        {
            Color c = Color.HSVToRGB(hsv.h, hsv.s, hsv.v, hdr);
            c.a = hsv.a;
            return c;
        }
    }
}