using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class TransformSizeTarget : AdaptorBlendTarget<float, Vector3, Vector3Blender>
    {
        protected override Vector3 Reference => transform.localScale;
        protected override Vector3 Adapt(float value) => Vector3.one * value;
        protected override void ApplyBlended(Vector3 value) => transform.localScale = value;
    }
}