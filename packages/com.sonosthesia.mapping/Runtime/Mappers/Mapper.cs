using UnityEngine;

namespace Sonosthesia.Mapping
{
    public abstract class Mapper<TSource, TTarget> : MonoBehaviour where TSource : struct where TTarget : struct
    {
        public abstract TTarget Map(TSource source, TSource reference, float timeOffset);
    }
}