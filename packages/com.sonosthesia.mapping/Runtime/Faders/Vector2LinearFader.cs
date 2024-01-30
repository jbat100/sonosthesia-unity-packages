using UnityEngine;

namespace Sonosthesia.Mapping
{
    public class Vector2LinearFader : LinearFader<Vector2>
    {
        protected override Vector2 LerpUnclamped(Vector2 start, Vector2 end, float value) => Vector2.LerpUnclamped(start, end, value);
    }
}