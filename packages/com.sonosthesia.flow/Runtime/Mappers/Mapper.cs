using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class Mapper<TSource, TTarget> : MonoBehaviour
    {
        public abstract TTarget Map(TSource source, TSource reference, float timeOffset);
    }
}