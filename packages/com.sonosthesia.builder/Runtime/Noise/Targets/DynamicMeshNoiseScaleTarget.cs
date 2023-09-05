using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class DynamicMeshNoiseScaleTarget : DynamicMeshNoiseTarget<Vector3, Vector3Blender>
    {
        protected override Vector3 Reference => Domain.scale;

        protected override void ApplyBlended(Vector3 value)
        {
            SpaceTRS domain = Domain;
            domain.scale = value;
            Domain = domain;
        }
    }
}