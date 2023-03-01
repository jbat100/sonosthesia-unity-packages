using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class TransformScaleTarget : Target<Vector3>
    {
        private Vector3 _reference;
        
        protected void Awake()
        {
            _reference = transform.localScale;
        }
        
        protected override void Apply(Vector3 value)
        {
            transform.localScale = TargetBlend.Blend(_reference, value);
        }
    }
}