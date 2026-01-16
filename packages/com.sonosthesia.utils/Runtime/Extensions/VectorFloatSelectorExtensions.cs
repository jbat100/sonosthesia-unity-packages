using UnityEngine;

namespace Sonosthesia.Utils
{
    public enum VectorFloatSelector
    {
        None,
        Magnitude,
        SqrMagnitude,
        Max,
        Min,
        Average,
        Sum
    }
    
    public static class VectorFloatSelectorExtensions
    {
        public static float Select(this Vector3 vector, VectorFloatSelector selector)
        {
            return selector switch
            {
                VectorFloatSelector.Magnitude => vector.magnitude,
                VectorFloatSelector.SqrMagnitude => vector.sqrMagnitude,
                VectorFloatSelector.Max => vector.Max(),
                VectorFloatSelector.Min => vector.Min(),
                VectorFloatSelector.Average => vector.Average(),
                VectorFloatSelector.Sum => vector.Sum(),
                _ => 0f
            };
        }
        
        public static float Select(this Vector2 vector, VectorFloatSelector selector)
        {
            return selector switch
            {
                VectorFloatSelector.Magnitude => vector.magnitude,
                VectorFloatSelector.SqrMagnitude => vector.sqrMagnitude,
                VectorFloatSelector.Max => vector.Max(),
                VectorFloatSelector.Min => vector.Min(),
                VectorFloatSelector.Average => vector.Average(),
                VectorFloatSelector.Sum => vector.Sum(),
                _ => 0f
            };
        }
    }
}