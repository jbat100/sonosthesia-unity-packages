using UnityEngine;

namespace Sonosthesia.Generator
{
    // For quick and dirty tests
    
    public class OscillatePosition : MonoBehaviour
    {
        [SerializeField] private PrimitiveOscillationType _oscillationType = PrimitiveOscillationType.Sine;
        [SerializeField] private Vector3 _direction = Vector3.up;
        [SerializeField] private float _frequency = 1f;
        [SerializeField] private bool _fixedUpdate = true;

        protected virtual void Update()
        {
            if (!_fixedUpdate)
            {
                Apply();
            }
        }
        
        protected virtual void FixedUpdate()
        {
            if (_fixedUpdate)
            {
                Apply();
            }
        }

        protected virtual void Apply()
        {
            float oscillation = PrimitiveOscillation.Evaluate(_oscillationType, Time.time * _frequency);
            transform.localPosition = _direction * oscillation;            
        }
    }
}

