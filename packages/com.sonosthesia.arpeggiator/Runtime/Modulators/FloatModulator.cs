using UnityEngine;

namespace Sonosthesia.Arpeggiator
{
    public class FloatModulator : BlendFloatModulator
    {
        [SerializeField] private FloatModulation _modulation;
        
        protected override float Modulate(float offset)
        {
            return _modulation.Modulate(offset);
        }
    }
}