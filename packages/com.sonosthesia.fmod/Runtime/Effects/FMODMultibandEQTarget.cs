using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public class FMODMultibandEQTarget : Target<float>
    {
        [SerializeField] private FMODMultibandEQ _eq;
        
        [SerializeField] private FMODBandEQ _band;
        
        [SerializeField] private FMODBandEQParameter _parameter;
        
        protected override void Apply(float value)
        {
            _eq.SetParameter(_band, _parameter, value);
        }
    }
}