namespace Sonosthesia.Interaction
{
    public readonly struct ValueEvent<TValue, TEvent> : IInteractionEvent
        where TValue : struct 
        where TEvent : struct, IInteractionEvent
    {
        public readonly TValue Value;
        public readonly TEvent Event;
        
        public IInteractionEndpoint Source => Event.Source;
        public IInteractionEndpoint Actor => Event.Actor;
        
        public ValueEvent(TValue v, TEvent e)
        {
            Value = v;
            Event = e;
        }

    }
}