using Sonosthesia.Target;
using UnityEngine.VFX;

namespace Sonosthesia.VFX
{
    public class FloatVFXTarget : VFXTarget<float, FloatBlender>
    {
        protected override void Apply(float value, VisualEffect visualEffect, int nameID)
        {
            visualEffect.SetFloat(nameID, value);
        }

        protected override float ExtractReference(VisualEffect visualEffect, int nameID)
        {
            return visualEffect.GetFloat(nameID);
        }
    }
}