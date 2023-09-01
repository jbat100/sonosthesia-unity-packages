using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class DynamicMeshNoisePositionTarget : DynamicMeshNoiseTarget<Vector3>
    {
        private Vector3 _reference;

        protected void Awake()
        {
            _reference = Domain.translation;
        }

        protected override void Apply(Vector3 value)
        {
            SpaceTRS domain = Domain;
            domain.translation = TargetBlend.Blend(_reference, (Vector3)value);
            Domain = domain;
        }
    }
}