using UnityEngine;

namespace Sonosthesia.Mapping
{
    public class ProviderMapper<TSource, TTarget> : Mapper<TSource, TTarget> where TSource : struct where TTarget : struct
    {
        [SerializeField] private ValueProvider<TTarget> _provider;

        public override TTarget Map(TSource source, TSource reference, float timeOffset)
        {
            return _provider.Value;
        }
    }
}