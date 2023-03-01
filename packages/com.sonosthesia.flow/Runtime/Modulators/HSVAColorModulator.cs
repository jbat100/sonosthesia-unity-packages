using UnityEngine;

namespace Sonosthesia.Flow
{
    public class HSVAColorModulator : Modulator<Color>
    {
        [SerializeField] private bool _hdr;
        
        [SerializeField] private FloatModulator _h;
        [SerializeField] private FloatModulator _s;
        [SerializeField] private FloatModulator _v;
        [SerializeField] private FloatModulator _a;
        
        public override Color Modulate(Color original, float offset)
        {
            Color.RGBToHSV(original, out float h, out float s, out float v);
            h = _h ? _h.Modulate(h, offset) : h;
            s = _s ? _s.Modulate(s, offset) : s;
            v = _v ? _v.Modulate(v, offset) : v;
            Color result = Color.HSVToRGB(h, s, v, _hdr);
            result.a = _a ? _a.Modulate(original.a, offset) : original.a;
            return result;
        }
    }
}