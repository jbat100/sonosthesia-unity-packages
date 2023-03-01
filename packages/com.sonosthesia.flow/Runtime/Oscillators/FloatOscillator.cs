using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class FloatOscillator : Oscillator<float>
    {
        [SerializeField] private float _timeScale = 1f;
        
        [SerializeField] private float _pause = 0f;
        
        [SerializeField] private float _amplitude = 1f;
        
        [SerializeField] private float _offset = 0f;

        public sealed override float Evaluate(float time)
        {
            float duration = Duration() * _timeScale;
            float period = duration + _pause;
            float iterationTime = (time + _offset) % period;
            if (iterationTime < duration)
            {
                return EvaluateIteration(iterationTime / _timeScale) * _amplitude;
            }
            return 0f;
        }

        protected abstract float Duration();
        
        protected abstract float EvaluateIteration(float time);
    }
}