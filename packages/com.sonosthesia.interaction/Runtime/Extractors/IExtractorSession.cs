namespace Sonosthesia.Interaction
{
    public interface IExtractorSession<in TEvent, TValue> where TEvent : IInteractionEvent where TValue : struct
    {
        public bool Setup(TEvent interactionEvent, out TValue value);
        public bool Update(TEvent interactionEvent, out TValue value);
    }
}