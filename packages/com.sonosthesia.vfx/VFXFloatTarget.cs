using UnityEngine.VFX;

namespace Sonosthesia
{
    public class VFXFloatTarget : VFXTarget<float>
    {
        protected override void Apply(float value, VisualEffect visualEffect, string nameID)
        {
            visualEffect.SetFloat(nameID, value);
        }
    }
}