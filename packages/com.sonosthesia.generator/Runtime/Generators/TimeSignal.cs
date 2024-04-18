    using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Generator
{
    /// <summary>
    /// Allows for smoothly changing time warp, simplification of TimeFloatGenerator + FloatGeneratorSignal
    /// Often used for controlling time warp in VFX/Shader graph
    /// </summary>
    public class TimeSignal : Signal<float>
    {
        [SerializeField] private float _timeFactor = 1f;
        
        private float _time;
        
        protected void OnEnable() => _time = 0;
        
        protected void Update()
        {
            _time += _timeFactor * Time.deltaTime;
            Broadcast(_time);
        }
    }
}