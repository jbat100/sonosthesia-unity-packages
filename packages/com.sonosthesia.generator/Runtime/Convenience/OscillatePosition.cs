using UnityEngine;

namespace Sonosthesia.Generator
{
    // For quick and dirty tests, could do same things with generators and targets 
    
    public class OscillatePosition : OscillateConvenience
    {
        [SerializeField] private Vector3 _direction = Vector3.up;
       
        protected override void Apply(float oscillation)
        {
            transform.localPosition = _direction * oscillation;            
        }
    }
}

