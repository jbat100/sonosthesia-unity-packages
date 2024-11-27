namespace Sonosthesia.Interaction
{
    public class ValueAffordance<TValue, TEvent> : InteractionAffordance<ValueEvent<TValue, TEvent>>
        where TValue : struct
        where TEvent : struct, IInteractionEvent
    {
        
    }
}