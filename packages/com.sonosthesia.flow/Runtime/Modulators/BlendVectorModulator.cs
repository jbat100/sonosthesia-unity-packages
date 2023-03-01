using System;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class BlendVectorModulator : Modulator<Vector3>
    {
        private enum Blend
        {
            Override,
            Add,
            Cross
        }

        [SerializeField] private Blend _blend;
        
        protected abstract Vector3 Modulate(float offset);
        
        public override Vector3 Modulate(Vector3 original, float offset)
        {
            Vector3 modulation = Modulate(offset);
            return _blend switch
            {
                Blend.Override => modulation,
                Blend.Add => original + modulation,
                Blend.Cross => Vector3.Cross(original, modulation),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}