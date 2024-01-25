using UnityEngine;

namespace Sonosthesia.Target
{
    public class ColorMaterialTarget : MaterialTarget<Color>
    {
        protected override string DefaultName => "_BaseColor";

        protected override void Apply(Color value, Material material)
        {
            material.SetColor(Name, value);
        }
    }
}