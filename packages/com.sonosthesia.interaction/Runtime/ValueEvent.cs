namespace Sonosthesia.Interaction
{
    public readonly struct ValueEvent<TValue, TEvent> 
        where TValue : struct 
        where TEvent : struct, IInteractionEvent
    {
        public readonly TValue Value;
        public readonly TEvent Event;

        public ValueEvent(TValue v, TEvent e)
        {
            Value = v;
            Event = e;
        }
    }
}