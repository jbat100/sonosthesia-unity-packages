using UnityEngine;

namespace Sonosthesia.Target
{
    public class TransformScaleTarget : BlendTarget<Vector3, Vector3Blender>
    {
        protected override Vector3 Reference => transform.localScale;

        protected override void ApplyBlended(Vector3 value) => transform.localScale = value;
    }
}