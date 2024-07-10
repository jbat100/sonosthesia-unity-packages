using UnityEngine;

namespace Sonosthesia.Target
{
    public class FloatMaterialTarget : MaterialTarget<float>
    {
        protected override bool CheckTarget(Material material) => material.HasFloat(NameID);

        protected override void Apply(float value, Material material) => material.SetFloat(NameID, value);

        protected override void ConfigureBlock(float value, MaterialPropertyBlock block) => block.SetFloat(NameID, value);
    }
}