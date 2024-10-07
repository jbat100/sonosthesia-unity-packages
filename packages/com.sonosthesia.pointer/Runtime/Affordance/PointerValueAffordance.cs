using Sonosthesia.Interaction;

namespace Sonosthesia.Pointer
{
    public class PointerValueAffordance<TValue> : 
        ValueAffordance<TValue, PointerEvent, PointerValueEventStreamContainer<TValue>> 
        where TValue : struct
    {
        
    }
}