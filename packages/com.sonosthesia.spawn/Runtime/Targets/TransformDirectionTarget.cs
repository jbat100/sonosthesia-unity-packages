using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class TransformDirectionTarget : AdaptorBlendTarget<float, Vector3, Vector3Blender>
    {
        [SerializeField] private Vector3 _direction;

        protected override Vector3 Reference => transform.localPosition;
        
        protected override Vector3 Adapt(float value) => _direction * value;

        protected override void ApplyBlended(Vector3 value) => transform.localPosition = value;
    }
}


