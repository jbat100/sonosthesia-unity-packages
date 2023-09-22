using UnityEngine;

namespace Sonosthesia.Utils
{
    // The API is difficult to get right because normalized doesn't necessarily mean anything, in the case of
    // noise motion for example. Although arguably noisy motion is not a path.
    
    public abstract class Path : MonoBehaviour
    {
        private float? _pathLength;

        protected abstract float ComputePathLength();

        private float PathLength
        {
            get
            {
                _pathLength ??= ComputePathLength();
                return _pathLength.Value;
            }
        }

        public void ResetPathLength() => _pathLength = null;

        public Vector3 GetPosition(float distance, bool isNormalized)
        {
            return GetPosition(isNormalized ? distance : distance / PathLength);
        }

        public Quaternion GetRotation(float distance, bool isNormalized)
        {
            return GetRotation(isNormalized ? distance : distance / PathLength);
        }
        
        public bool Evaluate(float distance, bool isNormalized, out Vector3 position, out Quaternion rotation)
        {
            return Evaluate(isNormalized ? distance : distance / PathLength, out position, out rotation);
        }

        protected abstract Vector3 GetPosition(float normalized);
        
        protected abstract Quaternion GetRotation(float normalized);

        protected abstract bool Evaluate(float normalized, out Vector3 position, out Quaternion rotation);
    }

    public static class PathExtensions
    {
        public static Trans Trans(this Path path, float distance, bool normalized)
        {
            return new Trans(path.GetPosition(distance, normalized), path.GetRotation(distance, normalized));
        }
    }
}

