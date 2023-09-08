using UnityEngine;

namespace Sonosthesia.Flow
{
    public class FloatGeneratorSignal : Signal<float>
    {
        [SerializeField] private Generator<float> _generator;

        [SerializeField] private float _timeFator = 1f;
        
        private float _time;
        
        protected void OnEnable() => _time = 0;
        
        protected void Update()
        {
            _time += _timeFator * Time.deltaTime;
            Broadcast(_generator.Evaluate(_time));
        }
    }
}