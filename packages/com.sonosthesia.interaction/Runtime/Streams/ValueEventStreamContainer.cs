using Sonosthesia.Utils;

namespace Sonosthesia.Interaction
{
    public class ValueEventStreamContainer<TValue, TEvent> : StreamContainer<ValueEvent<TValue, TEvent>>
        where TValue : struct
        where TEvent : struct
    {

    }
}