using System;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Utils
{
    [Serializable]
    public class ClampSettings
    {
        [SerializeField] private bool _clamp;
        public bool Clamp => _clamp;
        
        [SerializeField] private float _min = 0f;
        [SerializeField] private float _max = 1f;

        public float Process(float value)
        {
            return _clamp ? math.clamp(value, _min, _max) : value;
        }
    }
}