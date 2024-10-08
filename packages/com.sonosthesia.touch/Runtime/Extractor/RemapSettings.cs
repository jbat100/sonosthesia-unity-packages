using System;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Touch
{
    // TODO: could merge with the remap processor in Sonosthesia.Processing but breaks inheritance for dynamic processors
    
    [Serializable]
    public class RemapSettings
    {
        [SerializeField] private float _fromMin;
        [SerializeField] private float _fromMax = 1f;
        [SerializeField] private float _toMin;
        [SerializeField] private float _toMax = 1f;
        [SerializeField] private bool _clamp;

        public float Remap(float value)
        {
            if (Math.Abs(_fromMax - _fromMin) < 1e-6)
            {
                return _toMin;
            }
            float t = math.unlerp(_fromMin, _fromMax, value);
            float result = math.lerp(_toMin, _toMax, t);
            if (_clamp)
            {
                result = math.clamp(result, math.min(_toMin, _toMax), math.max(_toMin, _toMax));
            }

            return result;
        }
    }
}