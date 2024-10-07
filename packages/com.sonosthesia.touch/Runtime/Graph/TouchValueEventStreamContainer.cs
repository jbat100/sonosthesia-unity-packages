using Sonosthesia.Interaction;
using Sonosthesia.Utils;

namespace Sonosthesia.Touch
{
    public class TouchValueEventStreamContainer<TValue> : StreamContainer<ValueEvent<TValue, TouchEvent>>
        where TValue : struct
    {
        
    }
}