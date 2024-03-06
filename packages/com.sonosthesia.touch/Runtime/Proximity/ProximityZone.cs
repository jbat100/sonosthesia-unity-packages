using UnityEngine;

namespace Sonosthesia.Touch
{
    /// <summary>
    /// Used with collider trigger to (de)activate 
    /// </summary>
    
    public abstract class ProximityZone : Component
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point">Point in world space</param>
        /// <param name="closest">Point which is considered zone target for affordances</param>
        /// <returns></returns>
        public abstract bool ComputeTarget(Vector3 point, out Vector3 target);
    }
}