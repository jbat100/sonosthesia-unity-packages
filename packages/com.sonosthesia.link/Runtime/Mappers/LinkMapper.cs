using UnityEngine;

namespace Sonosthesia.Link
{
    public abstract class LinkMapper<TSource, TTarget> : MonoBehaviour where TSource : struct where TTarget : struct
    {
        public abstract TTarget Map(TSource source, TSource reference, float timeOffset);
    }
}