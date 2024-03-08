using UnityEngine;

namespace Sonosthesia.Proximity
{
    public abstract class ProximityZone : MonoBehaviour
    {
        [SerializeField] private float _margin;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point">Point in world space</param>
        /// <param name="target">Point which is considered zone target for affordances</param>
        /// <returns></returns>
        public bool ComputeTarget(Vector3 point, out Vector3 target)
        {
            if (!ComputeRawTarget(point, out Vector3 rawTarget))
            {
                target = default;
                return false;
            }

            target = Vector3.MoveTowards(rawTarget, point, _margin);
            return true;
        }
        
        protected abstract bool ComputeRawTarget(Vector3 point, out Vector3 target);
    }
}