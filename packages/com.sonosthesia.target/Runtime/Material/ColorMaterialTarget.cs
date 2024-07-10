using UnityEngine;

namespace Sonosthesia.Target
{
    public class ColorMaterialTarget : MaterialTarget<Color>
    {
        protected override string DefaultName => "_BaseColor";

        protected override bool CheckTarget(Material material) => material.HasColor(NameID);

        protected override void Apply(Color value, Material material) => material.SetColor(NameID, value);

        protected override void ConfigureBlock(Color value, MaterialPropertyBlock block) => block.SetColor(NameID, value);
    }
}