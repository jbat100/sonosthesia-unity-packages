using UnityEngine;

namespace Sonosthesia.Target
{
    public class TransformSizeTarget : AdaptorBlendTarget<float, Vector3, Vector3Blender>
    {
        [SerializeField] private Vector3 _scaleFactor = Vector3.one;
        
        protected override Vector3 Reference => transform.localScale;
        protected override Vector3 Adapt(float value) => _scaleFactor * value;
        protected override void ApplyBlended(Vector3 value) => transform.localScale = value;
    }
}