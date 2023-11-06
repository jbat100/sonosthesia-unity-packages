using UnityEngine;

namespace Sonosthesia.Target
{
    public class TransformPositionTarget : BlendTarget<Vector3, Vector3Blender>
    {
        protected override Vector3 Reference => transform.localPosition;

        protected override void ApplyBlended(Vector3 value) => transform.localPosition = value;

    }
}