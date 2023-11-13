namespace Sonosthesia.Noise
{
    public interface ISummable<T>
    {
        T Sum(T term);
    }
}