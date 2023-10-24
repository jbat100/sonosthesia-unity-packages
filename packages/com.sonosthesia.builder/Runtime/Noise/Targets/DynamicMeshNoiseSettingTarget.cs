using System;
using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class DynamicMeshNoiseSettingTarget : DynamicMeshNoiseTarget<float, FloatBlender>
    {
        private enum TargetType
        {
            Velocity,
            Displacement
        }

        [SerializeField] private TargetType _targetType;

        protected override float Reference => _targetType switch
        {
            TargetType.Velocity => DynamicSettings.Velocity,
            TargetType.Displacement => DynamicSettings.Displacement,
            _ => throw new ArgumentOutOfRangeException()
        };

        protected override void ApplyBlended(float value)
        {
            switch (_targetType)
            {
                case TargetType.Velocity:
                    DynamicSettings.Velocity = value;
                    break;
                case TargetType.Displacement:
                    DynamicSettings.Displacement = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}