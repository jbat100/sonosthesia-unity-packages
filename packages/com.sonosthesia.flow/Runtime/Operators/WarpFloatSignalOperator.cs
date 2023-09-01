using UnityEngine;

namespace Sonosthesia.Flow
{
    public class WarpFloatSignalOperator : SimpleOperator<float>
    {
        [SerializeField] private float _offset;
        
        [SerializeField] private float _scale = 1f;

        protected override float Process(float input) => Mathf.Lerp(0f, 1f, input) * _scale + _offset;
    }
}