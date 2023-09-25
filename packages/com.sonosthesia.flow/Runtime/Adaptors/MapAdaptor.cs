using System;
using UniRx;

namespace Sonosthesia.Flow
{
    public abstract class MapAdaptor<TSource, TTarget> : Adaptor<TSource, TTarget> where TTarget : struct where TSource : struct
    {
        protected abstract TTarget Map(TSource source);
        
        protected sealed override IDisposable Setup(Signal<TSource> source)
        {
            return source.SignalObservable.Subscribe(value => {
                TTarget mapped = Map(value);
                Broadcast(mapped);
            });
        }
    }
}