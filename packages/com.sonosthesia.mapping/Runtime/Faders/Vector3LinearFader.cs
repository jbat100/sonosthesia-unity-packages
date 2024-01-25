using UnityEngine;

namespace Sonosthesia.Mapping
{
    public class Vector3LinearFader : LinearFader<Vector3>
    {
        protected override Vector3 Lerp(Vector3 start, Vector3 end, float value) => Vector3.Lerp(start, end, value);
    }
}