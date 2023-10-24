using UnityEngine;

namespace Sonosthesia.Flow
{
    public class ColorGradientFader : Fader<Color>
    {
        [SerializeField] private Gradient _gradient;
        
        public override Color Fade(float fade)
        {
            return _gradient.Evaluate(fade);
        }
    }
}