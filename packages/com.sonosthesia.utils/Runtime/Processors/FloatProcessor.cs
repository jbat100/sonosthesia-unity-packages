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
        
        public float Process(float value)
        {
            return _offset + (_abs ? Mathf.Abs(value) : value) * _scale;
        }
    }
}