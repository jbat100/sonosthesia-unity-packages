using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class TransformRotationTarget : Target<Quaternion>
    {
        protected override void Apply(Quaternion value) => transform.rotation = value;
    }
}