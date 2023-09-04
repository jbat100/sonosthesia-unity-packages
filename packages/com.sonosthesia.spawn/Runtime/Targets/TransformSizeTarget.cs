using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class TransformSizeTarget : Target<float>
    {
        private float _reference;

        protected override void Awake()
        {
            base.Awake();
            _reference = transform.localScale.x;
        }
        
        protected override void Apply(float value)
        {
            transform.localScale = Vector3.one * TargetBlend.Blend(_reference, value);
        }
    }
}