namespace Sonosthesia.Interaction
{
    public interface IValueEvent<out TValue> where TValue : struct
    {
        TValue GetValue();
    }
}