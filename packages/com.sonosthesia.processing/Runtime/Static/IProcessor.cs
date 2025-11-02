using System;

namespace Sonosthesia.Processing
{
    public interface IProcessor<T> where T : struct
    {
        T Process(T value);
    }

    [Serializable]
    public class PassthroughProcessor<T> : IProcessor<T> where T : struct
    {
        public T Process(T value) => value;
    }
}