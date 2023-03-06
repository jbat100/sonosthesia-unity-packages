using UnityEngine;

namespace Sonosthesia.Utils
{
    public abstract class Path : MonoBehaviour
    {
        public abstract Vector3 Position(float distance, bool normalized);
    
        public abstract Quaternion Rotation(float distance, bool normalized);
    }

    public static class PathExtensions
    {
        public static Trans Trans(this Path path, float distance, bool normalized)
        {
            return new Trans(path.Position(distance, normalized), path.Rotation(distance, normalized));
        }
    }
}

