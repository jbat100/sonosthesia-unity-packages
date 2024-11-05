using System;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Utils
{
    [Serializable]
    public class ClampSettings
    {
        [SerializeField] private bool _clamp;
        [SerializeField] private float _min = 0f;
        [SerializeField] private float _max = 1f;

        public float Clamp(float value)
        {
            return _clamp ? math.clamp(value, _min, _max) : value;
        }
    }
}