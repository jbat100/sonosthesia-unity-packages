using Sonosthesia.Interaction;
using Sonosthesia.Channel;

namespace Sonosthesia.Pointer
{
    public class PointerValueEventChannel<TValue> : Channel<ValueEvent<TValue, PointerEvent>> where TValue : struct
    {
        
    }
}