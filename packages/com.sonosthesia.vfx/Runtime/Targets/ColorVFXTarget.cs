using UnityEngine;
using UnityEngine.VFX;
using Sonosthesia.Target;

namespace Sonosthesia.VFX
{
    public class ColorVFXTarget : VFXTarget<Color, ColorBlender>
    {
        protected override void Apply(Color value, VisualEffect visualEffect, int nameID)
        {
            visualEffect.SetVector4(nameID, value);
        }

        protected override Color ExtractReference(VisualEffect visualEffect, int nameID)
        {
            return visualEffect.GetVector4(nameID);
        }
    }
}