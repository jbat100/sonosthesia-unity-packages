using System;
using Sonosthesia.Ease;
using UnityEngine;

namespace Sonosthesia.Deform
{
    [Serializable]
    public class PlaneDeformationMaskSettings
    {
        [SerializeField] private bool _active;
        public bool Active => _active && _fade > 1e-3f;
        
        [SerializeField] [Range(0f, 0.5f)] private float _fade = 0.1f;
        public float Fade => _fade;

        [SerializeField] private EaseType _ease = EaseType.easeInOutSine;
        public EaseType Ease => _ease;
    }
}