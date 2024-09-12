using UnityEngine;

namespace Sonosthesia.Target
{
    public class IntMaterialTarget : MaterialTarget<int>
    {
        protected override bool CheckTarget(Material material) => material.HasInt(NameID);
        
        protected override void Apply(int value, Material material) => material.SetInt(NameID, value);

        protected override void ConfigureBlock(int value, MaterialPropertyBlock block) => block.SetInt(NameID, value);
    }
}