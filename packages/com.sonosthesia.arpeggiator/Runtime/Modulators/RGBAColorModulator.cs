using UnityEngine;

namespace Sonosthesia.Arpeggiator
{
    public class RGBAColorModulator : Modulator<Color>
    {
        [SerializeField] private FloatModulator _r;
        [SerializeField] private FloatModulator _g;
        [SerializeField] private FloatModulator _b;
        [SerializeField] private FloatModulator _a;
        
        public override Color Modulate(Color original, float offset)
        {
            return new Color(_r ? _r.Modulate(original.r, offset) : original.r,
                _g ? _g.Modulate(original.g, offset) : original.g,
                _b ? _b.Modulate(original.b, offset) : original.b,
                _a ? _a.Modulate(original.a, offset) : original.a);
        }
    }
}