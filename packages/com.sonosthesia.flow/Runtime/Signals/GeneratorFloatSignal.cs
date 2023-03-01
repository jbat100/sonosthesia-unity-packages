using UnityEngine;

namespace Sonosthesia.Flow
{
    public class GeneratorFloatSignal : Signal<float>
    {
        [SerializeField] private Generator<float> _generator;

        private float? _startTime;
        
        protected void OnEnable() => _startTime ??= Time.time;
        
        protected void OnDisable() => _startTime = null;

        protected void Update()
        {
            if (_startTime.HasValue)
            {
                Broadcast(_generator.Evaluate(Time.time - _startTime.Value));
            }
        }
    }
}