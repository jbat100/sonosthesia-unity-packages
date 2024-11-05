using System;
using UnityEngine;

namespace Sonosthesia.Utils
{
    [Serializable]
    public class RemapSettings
    {
        [SerializeField] private float _fromMin;
        [SerializeField] private float _fromMax = 1f;
        [SerializeField] private float _toMin;
        [SerializeField] private float _toMax = 1f;

        public float Remap(float value)
        {
            return MathUtils.Remap(value, _fromMin, _fromMax, _toMin, _toMax, false);
        }
    }
}