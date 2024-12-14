using System;
using Sonosthesia.Ease;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    [Serializable]
    public struct FloatBinsSettings
    {
        [SerializeField] private float _size;
        public float Size => _size;
        
        [SerializeField] private EaseType _ease;
        public EaseType Ease => _ease;
        
        [SerializeField] private FloatRange _transition;
        public FloatRange Transition => _transition; 
        
        [SerializeField] private FloatRange _velocity;
        public FloatRange Velocity => _velocity;
    }
}