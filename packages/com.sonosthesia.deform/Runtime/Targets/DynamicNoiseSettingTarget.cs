using System;
using UnityEngine;
using Sonosthesia.Target;

namespace Sonosthesia.Deform
{
    public class DynamicNoiseSettingTarget : DynamicNoiseTarget<float, FloatBlender>
    {
        private enum TargetType
        {
            Velocity,
            Displacement
        }

        [SerializeField] private TargetType _targetType;

        protected override float Reference => _targetType switch
        {
            TargetType.Velocity => DynamicNoiseSettings.Velocity,
            TargetType.Displacement => DynamicNoiseSettings.Displacement,
            _ => throw new ArgumentOutOfRangeException()
        };

        protected override void ApplyBlended(float value)
        {
            switch (_targetType)
            {
                case TargetType.Velocity:
                    DynamicNoiseSettings.Velocity = value;
                    break;
                case TargetType.Displacement:
                    DynamicNoiseSettings.Displacement = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}