namespace Sonosthesia.Touch
{
    public interface IEventValue<out TValue> where TValue : struct
    {
        TValue GetValue();
    }
}