using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class DynamicMeshNoiseScaleTarget : DynamicMeshNoiseTarget<Vector3>
    {
        private Vector3 _reference;

        protected override void Awake()
        {
            base.Awake();
            _reference = Domain.scale;
        }

        protected override void Apply(Vector3 value)
        {
            SpaceTRS domain = Domain;
            domain.scale = TargetBlend.Blend(_reference, (Vector3)value);
            Domain = domain;
        }
    }
}