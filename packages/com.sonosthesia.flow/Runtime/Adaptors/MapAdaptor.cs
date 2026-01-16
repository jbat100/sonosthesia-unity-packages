using System;
using UniRx;
using Sonosthesia.Signal;

namespace Sonosthesia.Flow
{
    public abstract class MapAdaptor<TSource, TTarget> : Adaptor<TSource, TTarget> where TTarget : struct where TSource : struct
    {
        protected abstract TTarget Map(TSource source);
        
        protected sealed override IDisposable Setup(ISignal<TSource> source)
        {
            return source.Observable.Subscribe(value => {
                TTarget mapped = Map(value);
                Broadcast(mapped);
            });
        }
    }
}