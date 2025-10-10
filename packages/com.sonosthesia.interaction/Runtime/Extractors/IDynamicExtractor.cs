namespace Sonosthesia.Interaction
{
    public interface IDynamicExtractor<in TEvent, TValue> 
        where TEvent : IInteractionEvent where TValue : struct
    {
        IDynamicExtractorSession<TEvent, TValue> MakeSession();
    }
}