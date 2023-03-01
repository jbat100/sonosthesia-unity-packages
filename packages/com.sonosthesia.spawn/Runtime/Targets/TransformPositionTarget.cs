using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class TransformPositionTarget : Target<Vector3>
    {
        private Vector3 _reference;
        
        protected void Awake()
        {
            _reference = transform.localPosition;
        }
        
        protected override void Apply(Vector3 value)
        {
            transform.localPosition = TargetBlend.Blend(_reference, value);
        }
    }
}