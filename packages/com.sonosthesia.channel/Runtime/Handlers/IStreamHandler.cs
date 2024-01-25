using System;
using UniRx;

namespace Sonosthesia.Channel
{
    // consider moving to separate stream package or renaming IStreamHandler
    
    public interface IStreamHandler<in T> where T : struct
    {
        IObservable<Unit> HandleStream(IObservable<T> stream);
    }
}