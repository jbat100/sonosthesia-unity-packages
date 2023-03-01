using System;
using UniRx;

namespace Sonosthesia.Flow
{
    public abstract class ChannelStreamSimpleHandler<T> : Signal<T>, IChannelStreamHandler<T> where T : struct
    {
        public IObservable<Unit> HandleStream(IObservable<T> stream)
        {
            return stream.Do(Broadcast).AsUnitObservable();
        }
    }
}