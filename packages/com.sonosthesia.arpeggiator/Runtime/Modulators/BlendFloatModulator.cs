using System;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class BlendFloatModulator : Modulator<float>
    {
        private enum Blend
        {
            Override,
            Add,
            Multiply
        }
        
        [SerializeField] private Blend _blend;

        protected abstract float Modulate(float offset);
        
        public override float Modulate(float original, float offset)
        {
            float modulation = Modulate(offset);
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