using UnityEngine;

namespace Sonosthesia.Target
{
    public class FloatMaterialTarget : MaterialTarget<float>
    {
        protected override void Apply(float value, Material material) => material.SetFloat(NameID, value);
    }
}