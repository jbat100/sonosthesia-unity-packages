using Sonosthesia.Interaction;
using Sonosthesia.Utils;

namespace Sonosthesia.Pointer
{
    public class PointerValueEventStreamContainer<TValue> : StreamContainer<ValueEvent<TValue, PointerEvent>>
        where TValue : struct
    {
        
    }
}