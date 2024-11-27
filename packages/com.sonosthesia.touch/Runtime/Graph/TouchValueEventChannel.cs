using Sonosthesia.Interaction;
using Sonosthesia.Channel;

namespace Sonosthesia.Touch
{
    public class TouchValueEventChannel<TValue> : Channel<ValueEvent<TValue, TouchEvent>>
        where TValue : struct
    {
        
    }
}