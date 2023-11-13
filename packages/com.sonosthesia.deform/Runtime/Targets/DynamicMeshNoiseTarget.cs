using System;
using Sonosthesia.Noise;
using UnityEngine;
using Sonosthesia.Target;

namespace Sonosthesia.Deform
{
    public abstract class DynamicMeshNoiseTarget<T, B> : BlendTarget<T, B> 
        where T : struct where B : struct, IBlender<T>
    {
        [SerializeField] private MeshNoiseController _noiseController;
        
        [SerializeField] private int _componentIndex;

        // TODO : this is hacky, refactor as needs arise 
        
        protected DynamicSettings DynamicSettings => _noiseController switch
            {
                TriDomainMeshNoiseController triDomain => triDomain.GetSettings(_componentIndex).Settings,
                TriAdvancedMeshNoiseController triAdvanced => triAdvanced.GetSettings(_componentIndex),
                _ => throw new NotImplementedException()
            };

        protected SpaceTRS Domain
        {
            get
            {
                return _noiseController switch
                {
                    TriDomainMeshNoiseController triDomain => triDomain.GetSettings(_componentIndex).Domain,
                    TriAdvancedMeshNoiseController triAdvanced => triAdvanced.Domain,
                    _ => throw new NotImplementedException()
                };
            }
            set
            {
                switch (_noiseController)
                {
                    case TriDomainMeshNoiseController triDomain:
                        triDomain.GetSettings(_componentIndex).Domain = value;
                        break;
                    case TriAdvancedMeshNoiseController triAdvanced:
                        triAdvanced.Domain = value;
                        break;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (!_noiseController)
            {
                _noiseController = GetComponent<MeshNoiseController>();
            }
        }
    }
}