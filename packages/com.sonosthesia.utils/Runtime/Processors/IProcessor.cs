namespace Sonosthesia.Utils
{
    public interface IProcessor<T> where T : struct
    {
        T Process(T value);
    }

    public struct PassthroughProcessor<T> : IProcessor<T> where T : struct
    {
        public T Process(T value) => value;
    }
}