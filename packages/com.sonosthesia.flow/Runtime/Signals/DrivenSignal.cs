namespace Sonosthesia.Flow
{
    public class DrivenSignal<T> : Signal<T> where T : struct
    {
        public void Drive(T value)
        {
            Broadcast(value);
        }
    }
}