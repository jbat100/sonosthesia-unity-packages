using UnityEngine;
using UnityEngine.VFX;

namespace Sonosthesia.VFX
{
    public class VFXColorTarget : VFXTarget<Color>
    {
        protected override void Apply(Color value, VisualEffect visualEffect, string nameID)
        {
            visualEffect.SetVector4(nameID, value);
        }
    }
}