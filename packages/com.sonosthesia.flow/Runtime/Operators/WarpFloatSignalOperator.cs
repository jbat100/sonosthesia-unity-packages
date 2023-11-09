using UnityEngine;

namespace Sonosthesia.Signal
{
    public class WarpFloatSignalOperator : SimpleOperator<float>
    {
        [SerializeField] private float _offset;
        
        [SerializeField] private float _scale = 1f;

        protected override float Process(float input) => input * _scale + _offset;
    }
}