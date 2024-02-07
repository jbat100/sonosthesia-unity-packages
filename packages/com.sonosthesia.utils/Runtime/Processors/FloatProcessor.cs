using System;
using UnityEngine;

namespace Sonosthesia.Utils
{
    [Serializable]
    public class FloatProcessor
    {
        [SerializeField] private bool _abs;
        
        [SerializeField] private float _scale = 1f;

        [SerializeField] private float _offset;

        [SerializeField] private bool _clamp;

        [SerializeField] private Vector2 _clampRange;
        
        public float Process(float value)
        {
            float result = value;

            if (_abs)
            {
                result = _abs ? Mathf.Abs(value) : value;
            }
            
            result = _offset + result * _scale;

            if (_clamp)
            {
                result = Mathf.Clamp(result, _clampRange.x, _clampRange.y);
            }

            return result;
        }
    }
}