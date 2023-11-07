using System;
using UnityEngine;

namespace Sonosthesia.Builder
{
    #region inspector settings
        
    [Serializable]
    public class DynamicSettings
    {
        public int Frequency;
        public float Displacement;
        public float Velocity;
        public AnimationCurve LerpCurve;
    }
        
    [Serializable]
    public class DomainDynamicSettings
    {
        public DynamicSettings Settings;
        public SpaceTRS Domain = new () { scale = 1f };
    }
        
    #endregion 
}