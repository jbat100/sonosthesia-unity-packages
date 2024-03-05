using UnityEngine.VFX;

namespace Sonosthesia.VFX
{
    public class VFXFloatTarget : VFXTarget<float>
    {
        protected override void Apply(float value, VisualEffect visualEffect, int nameID)
        {
            visualEffect.SetFloat(nameID, value);
        }
    }
}