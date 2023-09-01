using System;
using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public abstract class DynamicMeshNoiseTarget<T> : Target<T> where T : struct
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
    }
}