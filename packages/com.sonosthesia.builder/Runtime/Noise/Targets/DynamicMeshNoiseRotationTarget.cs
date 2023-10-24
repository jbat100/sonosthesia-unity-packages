using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class DynamicMeshNoiseRotationTarget : DynamicMeshNoiseDomainTarget<Quaternion, QuaternionBlender>
    {
        protected override Quaternion Reference => Quaternion.Euler(Domain.rotation);

        protected override void ApplyBlended(Quaternion value)
        {
            SpaceTRS domain = Domain;
            domain.rotation = value.eulerAngles;
            Domain = domain;
        }
    }
}