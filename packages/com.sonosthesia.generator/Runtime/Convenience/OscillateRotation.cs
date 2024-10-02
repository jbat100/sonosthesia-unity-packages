using UnityEngine;

namespace Sonosthesia.Generator
{
    public class OscillateRotation : MonoBehaviour
    {
        [SerializeField] private PrimitiveOscillationType _oscillationType = PrimitiveOscillationType.Sine;
        [SerializeField] private Vector3 _axis = Vector3.up;
        [SerializeField] private float _degrees = 180f;
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
            transform.localRotation = Quaternion.AngleAxis(oscillation * _degrees, _axis);
        }
        
    }
}