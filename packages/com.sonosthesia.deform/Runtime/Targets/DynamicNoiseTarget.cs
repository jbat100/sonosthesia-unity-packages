using System;
using Sonosthesia.Noise;
using UnityEngine;
using Sonosthesia.Target;

namespace Sonosthesia.Deform
{
    public abstract class DynamicNoiseTarget<T, B> : BlendTarget<T, B> 
        where T : struct where B : struct, IBlender<T>
    {
        [SerializeField] private DynamicNoiseConfiguration _configuration;
        
        [SerializeField] private int _componentIndex;
        
        protected DynamicNoiseSettings DynamicNoiseSettings => _configuration.GetSettings(_componentIndex);
        
        protected override void Awake()
        {
            base.Awake();
            if (!_configuration)
            {
                _configuration = GetComponent<DynamicNoiseConfiguration>();
            }
        }
    }
}