using System;
using UniRx;
using Sonosthesia.Signal;

namespace Sonosthesia.Channel
{
    /// <summary>
    /// Useful to access a stream as a signal, note successive HandleStream will not cancel previous streams
    /// and they will overlap 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class StreamHandlerSignal<T> : Signal<T>, IStreamHandler<T> where T : struct
    {
        public IObservable<Unit> HandleStream(IObservable<T> stream)
        {
            return stream.Do(Broadcast).AsUnitObservable();
        }
    }
}