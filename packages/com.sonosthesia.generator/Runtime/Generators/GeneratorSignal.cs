using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Generator
{
    public class GeneratorSignal<T> : Signal<T> where T : struct
    {
        [SerializeField] private Generator<T> _generator;

        [SerializeField] private float _timeFactor = 1f;
        
        private float _time;
        
        protected void OnEnable() => ResetTime();
        
        protected void Update()
        {
            _time += _timeFactor * Time.deltaTime;
            T raw = _generator.Evaluate(_time);
            Broadcast(PostProcess(raw));
        }

        protected virtual T PostProcess(T value) => value;

        public void ResetTime() => _time = 0;
    }
}