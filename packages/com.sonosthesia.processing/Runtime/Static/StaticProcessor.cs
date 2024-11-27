namespace Sonosthesia.Processing
{
    public interface IStaticProcessor<T>
    {
        T Process(T input);
    }
}