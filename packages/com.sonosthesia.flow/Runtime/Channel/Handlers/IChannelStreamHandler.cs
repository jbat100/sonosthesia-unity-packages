using System;
using UniRx;

namespace Sonosthesia.Flow
{
    public interface IChannelStreamHandler<in T> where T : struct
    {
        IObservable<Unit> HandleStream(IObservable<T> stream);
    }
}