using Sonosthesia.Noise;
using Sonosthesia.Target;
using UnityEngine;

namespace Sonosthesia.Deform
{
    public class DynamicNoiseScaleTarget : DynamicNoiseTarget<Vector3, Vector3Blender>
    {
        protected override Vector3 Reference => DynamicNoiseSettings.Domain.scale;

        protected override void ApplyBlended(Vector3 value)
        {
            SpaceTRS domain = DynamicNoiseSettings.Domain;
            domain.scale = value;
            DynamicNoiseSettings.Domain = domain;
        }
    }
}