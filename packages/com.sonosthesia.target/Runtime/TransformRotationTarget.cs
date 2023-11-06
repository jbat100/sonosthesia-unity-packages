using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Target
{
    public class TransformRotationTarget : Target<Quaternion>
    {
        protected override void Apply(Quaternion value) => transform.rotation = value;
    }
}