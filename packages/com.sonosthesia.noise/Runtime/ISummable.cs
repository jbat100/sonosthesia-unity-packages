namespace Sonosthesia.Builder
{
    public interface ISummable<T>
    {
        T Sum(T term);
    }
}