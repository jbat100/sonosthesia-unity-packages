using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class DynamicMeshNoiseRotationTarget : DynamicMeshNoiseDomainTarget<Quaternion>
    {
        private Quaternion _reference;

        protected override void Awake()
        {
            base.Awake();
            _reference = Quaternion.Euler(Domain.rotation);
        }
        
        protected override void Apply(Quaternion value)
        {
            SpaceTRS domain = Domain;
            domain.rotation = TargetBlend.Blend(_reference, value).eulerAngles;
            Domain = domain;
        }
    }
}