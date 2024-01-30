using UnityEngine;

namespace Sonosthesia.Mapping
{
    public class Vector3LinearFader : LinearFader<Vector3>
    {
        protected override Vector3 LerpUnclamped(Vector3 start, Vector3 end, float value) => Vector3.LerpUnclamped(start, end, value);
    }
}