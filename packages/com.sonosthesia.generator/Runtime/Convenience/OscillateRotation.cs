using UnityEngine;

namespace Sonosthesia.Generator
{
    public class OscillateRotation : OscillateConvenience
    {
        [SerializeField] private Vector3 _axis = Vector3.up;
        [SerializeField] private float _degrees = 180f;
        
        protected override void Apply(float oscillation)
        {
            transform.localRotation = Quaternion.AngleAxis(oscillation * _degrees, _axis);
        }
        
    }
}