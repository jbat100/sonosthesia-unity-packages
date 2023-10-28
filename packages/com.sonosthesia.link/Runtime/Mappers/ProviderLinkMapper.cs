using Sonosthesia.Mapping;
using UnityEngine;

namespace Sonosthesia.Link
{
    public class ProviderLinkMapper<TSource, TTarget> : LinkMapper<TSource, TTarget> where TSource : struct where TTarget : struct
    {
        [SerializeField] private ValueProvider<TTarget> _provider;

        public override TTarget Map(TSource source, TSource reference, float timeOffset)
        {
            return _provider.Value;
        }
    }
}