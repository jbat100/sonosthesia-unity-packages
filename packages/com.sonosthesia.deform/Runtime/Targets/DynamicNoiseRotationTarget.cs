using Sonosthesia.Noise;
using Sonosthesia.Target;
using UnityEngine;

namespace Sonosthesia.Deform
{
    public class DynamicNoiseRotationTarget : DynamicNoiseTarget<Quaternion, QuaternionBlender>
    {
        protected override Quaternion Reference => Quaternion.Euler(DynamicNoiseSettings.Domain.rotation);

        protected override void ApplyBlended(Quaternion value)
        {
            SpaceTRS domain = DynamicNoiseSettings.Domain;
            domain.rotation = value.eulerAngles;
            DynamicNoiseSettings.Domain = domain;
        }
    }
}