namespace Sonosthesia.Touch
{
    public interface IValueEvent<out TValue> where TValue : struct
    {
        TValue GetValue();
    }
}