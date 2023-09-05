using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class DynamicMeshNoisePositionTarget : DynamicMeshNoiseTarget<Vector3, Vector3Blender>
    {
        protected override Vector3 Reference => Domain.translation;

        protected override void ApplyBlended(Vector3 value)
        {
            SpaceTRS domain = Domain;
            domain.translation = value;
            Domain = domain;
        }
    }
}