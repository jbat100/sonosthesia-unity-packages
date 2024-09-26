namespace Sonosthesia.Utils
{
    public interface ILerpable<T>
    {
        // 0 is self, 1 is other
        T Lerp(T other, float lerp);
    }
}