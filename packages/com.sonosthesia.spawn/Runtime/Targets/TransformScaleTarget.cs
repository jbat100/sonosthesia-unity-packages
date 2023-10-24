using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class TransformScaleTarget : BlendTarget<Vector3, Vector3Blender>
    {
        protected override Vector3 Reference => transform.localScale;

        protected override void ApplyBlended(Vector3 value) => transform.localScale = value;
    }
}