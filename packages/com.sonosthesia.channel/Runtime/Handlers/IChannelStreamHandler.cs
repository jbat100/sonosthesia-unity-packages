using System;
using UniRx;

namespace Sonosthesia.Channel
{
    public interface IChannelStreamHandler<in T> where T : struct
    {
        IObservable<Unit> HandleStream(IObservable<T> stream);
    }
}