namespace Sonosthesia.Interaction
{
    public interface IStaticExtractor<in TEvent, TValue>
        where TEvent : IInteractionEvent where TValue : struct
    {
        bool Extract(TEvent e, out TValue value);
    }
}