using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class TransformPositionTarget : BlendTarget<Vector3, Vector3Blender>
    {
        protected override Vector3 Reference => transform.localPosition;

        protected override void ApplyBlended(Vector3 value) => transform.localPosition = value;

    }
}