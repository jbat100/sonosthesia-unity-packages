using UnityEngine;

namespace Sonosthesia.Target
{
    public class Vector2MaterialTarget : MaterialTarget<Vector2>
    {
        protected override void Apply(Vector2 value, Material material) => material.SetVector(NameID, value);
    }
}