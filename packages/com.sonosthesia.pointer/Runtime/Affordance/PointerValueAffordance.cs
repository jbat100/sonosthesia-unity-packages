using Sonosthesia.Interaction;

namespace Sonosthesia.Pointer
{
    public class PointerValueAffordance<TValue> : 
        ValueAffordance<TValue, PointerValueEvent<TValue>, PointerValueEventStreamContainer<TValue>> 
        where TValue : struct
    {
        
    }
}