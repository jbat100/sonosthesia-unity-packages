using Sonosthesia.Noise;
using Sonosthesia.Target;
using UnityEngine;

namespace Sonosthesia.Deform
{
    public class DynamicNoisePositionTarget : DynamicNoiseTarget<Vector3, Vector3Blender>
    {
        protected override Vector3 Reference => DynamicNoiseSettings.Domain.translation;

        protected override void ApplyBlended(Vector3 value)
        {
            SpaceTRS domain = DynamicNoiseSettings.Domain;
            domain.translation = value;
            DynamicNoiseSettings.Domain = domain;
        }
    }
}