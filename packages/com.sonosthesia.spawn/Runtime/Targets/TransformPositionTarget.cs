using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class TransformPositionTarget : Target<Vector3>
    {
        private Vector3 _reference;
        
        protected override void Awake()
        {
            base.Awake();
            _reference = transform.localPosition;
        }
        
        protected override void Apply(Vector3 value)
        {
            transform.localPosition = TargetBlend.Blend(_reference, value);
        }
    }
}