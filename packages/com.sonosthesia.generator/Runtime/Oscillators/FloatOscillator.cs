using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Generator
{
    public abstract class FloatOscillator : Oscillator<float>
    {
        // TODO : consider moving up to generic Oscillator and have T EvaluateIteration(float time);
        
        [SerializeField] private float _timeScale = 1f;
        
        [SerializeField] private float _pause = 0f;

        [SerializeField] private float _offset = 0f;

        [SerializeField] private FloatProcessor _postProcessor;

        public sealed override float Evaluate(float time)
        {
            float duration = Duration * _timeScale;
            float period = duration + _pause;
            float iterationTime = (time + _offset) % period;
            if (iterationTime < duration)
            {
                return _postProcessor.Process(EvaluateIteration(iterationTime / _timeScale));
            }
            return _postProcessor.Process(0f);
        }

        protected abstract float Duration { get; }
        
        protected abstract float EvaluateIteration(float time);
    }
}