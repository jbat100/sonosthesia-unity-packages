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
        Sum,
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
                Vector3Selector.Max => vector.Max(),
                Vector3Selector.Min => vector.Min(),
                Vector3Selector.Average => vector.Average(),
                Vector3Selector.Sum => vector.Sum(),
                Vector3Selector.X => vector.x,
                Vector3Selector.Y => vector.y,
                Vector3Selector.Z => vector.z,
                _ => 0f
            };
        }
    }
}