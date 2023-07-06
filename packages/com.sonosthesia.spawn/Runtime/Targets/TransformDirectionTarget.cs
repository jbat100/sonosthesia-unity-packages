using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class TransformDirectionTarget : Target<float>
    {
        [SerializeField] private Vector3 _direction;
        
        private Vector3 _reference;
        
        protected void Awake()
        {
            _reference = transform.localPosition;
        }
        
        protected override void Apply(float value)
        {
            Vector3 displacement = _direction * value;
            transform.localPosition = TargetBlend.Blend(_reference, displacement);
        }
    }
}


