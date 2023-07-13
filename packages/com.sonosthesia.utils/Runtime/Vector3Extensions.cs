using UnityEngine;

namespace Sonosthesia.Utils
{
    public enum Vector3Selector
    {
        None,
        Magnitude,
        SqrMagnitude,
        Max,
        Min,
        Average,
        X,
        Y,
        Z
    }
    
    public static class Vector3Extensions
    {
        public static float Select(this Vector3 vector, Vector3Selector selector)
        {
            return selector switch
            {
                Vector3Selector.Magnitude => vector.magnitude,
                Vector3Selector.SqrMagnitude => vector.sqrMagnitude,
                Vector3Selector.Max => Mathf.Max(vector.x, vector.y, vector.z),
                Vector3Selector.Min => Mathf.Min(vector.x, vector.y, vector.z),
                Vector3Selector.Average => (vector.x + vector.y + vector.z) * 0.333333333333f,
                Vector3Selector.X => vector.x,
                Vector3Selector.Y => vector.y,
                Vector3Selector.Z => vector.z,
                _ => 0f
            };
        }
    }
}