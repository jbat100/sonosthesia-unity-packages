using FMODUnity;
using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public class FMODEmitterParamTarget : Target<float>
    {
        [SerializeField] private StudioEventEmitter _emitter;
        
        [SerializeField] private string _parameterName;
        
        protected override void Apply(float value)
        {
            if (!_emitter)
            {
                return;
            }
            
            _emitter.SetParameter(_parameterName, value);
        }
    }
}