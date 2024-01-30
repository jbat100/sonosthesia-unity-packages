using UnityEngine;

namespace Sonosthesia.Utils
{
    public static class HDRColor 
    {
        public static Color Make(float r, float g, float b, float a, float intensity)
        {
            float mul = Mathf.Pow(2, intensity);
            return new Color(r * mul, g * mul, b * mul, a);
        }
        
        public static Color Make(Color color, float a, float intensity) => Make(color.r, color.g, color.b, a, intensity);

        public static Color Make(Color color, float intensity) => Make(color.r, color.g, color.b, color.a, intensity);
    }
    
    public static class ColorExtensions
    {
        public static Color WithHDRIntensity(this Color color, float intensity)
        {
            return HDRColor.Make(color, intensity);
        }
    }
}