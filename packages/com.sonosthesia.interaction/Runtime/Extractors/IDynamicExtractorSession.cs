namespace Sonosthesia.Interaction
{
    public interface IDynamicExtractorSession<in TEvent, TValue> 
        where TEvent : IInteractionEvent where TValue : struct
    {
        public bool Setup(TEvent interactionEvent, out TValue value);
        public bool Update(TEvent interactionEvent, out TValue value);
    }

    public abstract class StatelessExtractorSession<TEvent, TValue> : IDynamicExtractorSession<TEvent, TValue> 
        where TEvent : IInteractionEvent where TValue : struct
    {
        protected abstract bool Extract(TEvent interactionEvent, out TValue value);
        
        public bool Setup(TEvent interactionEvent, out TValue value) => Extract(interactionEvent, out value);

        public bool Update(TEvent interactionEvent, out TValue value) => Extract(interactionEvent, out value);
    }
}