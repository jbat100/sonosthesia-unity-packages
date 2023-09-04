using System;
using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class DynamicMeshNoiseSettingTarget : DynamicMeshNoiseTarget<float>
    {
        private enum TargetType
        {
            Velocity,
            Displacement
        }

        [SerializeField] private TargetType _targetType;

        private float _reference;
        
        protected override void Awake()
        {
            base.Awake();
            _reference = _targetType switch
            {
                TargetType.Velocity => DynamicSettings.Velocity,
                TargetType.Displacement => DynamicSettings.Displacement,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        protected override void Apply(float value)
        {
            float result = TargetBlend.Blend(_reference, value);
            
            switch (_targetType)
            {
                case TargetType.Velocity:
                    DynamicSettings.Velocity = result;
                    break;
                case TargetType.Displacement:
                    DynamicSettings.Displacement = result;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}