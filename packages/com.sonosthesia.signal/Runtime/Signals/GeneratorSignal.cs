using UnityEngine;

namespace Sonosthesia.Signal
{
    public class GeneratorSignal<T> : Signal<T> where T : struct
    {
        [SerializeField] private Generator<T> _generator;

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