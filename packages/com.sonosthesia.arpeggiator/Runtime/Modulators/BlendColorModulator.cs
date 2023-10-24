using System;
using UnityEngine;

namespace Sonosthesia.Arpeggiator
{
    public abstract class BlendColorModulator : Modulator<Color>
    {
        private enum Blend
        {
            Override,
            Add,
            Multiply
        }
        
        [SerializeField] private Blend _blend;
        
        protected abstract Color Modulate(float offset);

        public override Color Modulate(Color original, float offset)
        {
            Color modulation = Modulate(offset);
            return _blend switch
            {
                Blend.Override => modulation,
                Blend.Add => original + modulation,
                Blend.Multiply => original * modulation,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}