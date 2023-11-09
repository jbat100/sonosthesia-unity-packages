using UnityEngine;

namespace Sonosthesia.Signal
{
    public class FloatGeneratorSignal : Signal<float>
    {
        [SerializeField] private Generator<float> _generator;

        [SerializeField] private float _timeFactor = 1f;
        
        private float _time;
        
        protected void OnEnable() => _time = 0;
        
        protected void Update()
        {
            _time += _timeFactor * Time.deltaTime;
            Broadcast(_generator.Evaluate(_time));
        }
    }
}