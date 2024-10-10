namespace Sonosthesia.Interaction
{
    public class ValueAffordance<TValue, TEvent> : AbstractAffordance<ValueEvent<TValue, TEvent>>
        where TValue : struct
        where TEvent : struct, IInteractionEvent
    {
        
    }
}