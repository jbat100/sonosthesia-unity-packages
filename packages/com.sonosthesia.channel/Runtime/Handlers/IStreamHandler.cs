using System;
using UniRx;

namespace Sonosthesia.Channel
{
    // consider moving to separate stream package or renaming IStreamHandler
    
    /// <summary>
    /// Can be used to query instantiated game objects for components which can handle a value stream (channel or other)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStreamHandler<in T> where T : struct
    {
        /// <summary>
        /// Returns a Unit observable which fires and completes when the handler is done, the is not
        /// necessarily at the same time as the handled stream, in the case of fade outs for example
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        IObservable<Unit> HandleStream(IObservable<T> stream);
    }
}