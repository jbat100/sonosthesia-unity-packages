using System;
using UnityEngine;

namespace Sonosthesia.TouchDeform
{
    public enum DynamicTrackingStrategy
    {
        FreezePosition,
        FreezeVelocity,
        Follow
    }
    
    [Serializable]
    public class DynamicTrackingSettings
    {
        [SerializeField] private DynamicTrackingStrategy _strategy;
        public DynamicTrackingStrategy Strategy => _strategy;

        [SerializeField] private float _drag;
        public float Drag => _drag;
    }
}