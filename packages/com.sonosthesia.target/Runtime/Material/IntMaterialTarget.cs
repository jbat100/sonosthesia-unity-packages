using UnityEngine;

namespace Sonosthesia.Target
{
    public class IntMaterialTarget : MaterialTarget<int>
    {
        protected override void Apply(int value, Material material) => material.SetInt(NameID, value);
    }
}