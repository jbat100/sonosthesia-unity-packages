using UnityEngine;

namespace Sonosthesia.Generator
{
    // For quick and dirty tests, could do same things with generators and targets 
    
    public abstract class OscillateConvenience : MonoBehaviour
    {
        [SerializeField] private PrimitiveOscillationType _oscillationType = PrimitiveOscillationType.Sine;
        [SerializeField] private float _frequency = 1f;
        [SerializeField] private bool _fixedUpdate = true;

        // tracking current time avoids discontinuities when disabling / re-enabling oscillator
        private float _currentTime;

        protected virtual void Update()
        {
            if (!_fixedUpdate)
            {
                _currentTime += Time.deltaTime * _frequency;
                Apply(PrimitiveOscillation.Evaluate(_oscillationType, _currentTime));
            }
        }

        protected virtual void FixedUpdate()
        {
            if (_fixedUpdate)
            {
                _currentTime += Time.fixedDeltaTime * _frequency;
                Apply(PrimitiveOscillation.Evaluate(_oscillationType, _currentTime));
            }
        }

        protected abstract void Apply(float time);
    }
}