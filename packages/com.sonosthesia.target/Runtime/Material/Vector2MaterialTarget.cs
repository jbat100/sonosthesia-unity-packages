using UnityEngine;

namespace Sonosthesia.Target
{
    public class Vector2MaterialTarget : MaterialTarget<Vector2>
    {
        protected override bool CheckTarget(Material material) => material.HasVector(NameID);
        
        protected override void Apply(Vector2 value, Material material) => material.SetVector(NameID, value);

        protected override void ConfigureBlock(Vector2 value, MaterialPropertyBlock block) => block.SetVector(NameID, value);
    }
}