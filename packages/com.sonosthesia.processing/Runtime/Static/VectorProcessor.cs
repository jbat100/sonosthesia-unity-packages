using System;
using UnityEngine;

namespace Sonosthesia.Processing
{
    [Flags]
    public enum VectorProcessingType
    {
        Normalize = 1 << 0,
        Scale = 1 << 2
    }

    public static class VectorProcessorExtensions
    {
        public static Vector3 ProcessVector(this VectorProcessingType processingType, Vector3 vector, float scale)
        {
            if (processingType.HasFlag(VectorProcessingType.Normalize))
            {
                vector = vector.normalized;
            }

            if (processingType.HasFlag(VectorProcessingType.Scale))
            {
                vector *= scale;
            }
            return vector;
        }
    }
        
}