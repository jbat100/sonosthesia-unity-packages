namespace Sonosthesia.Interaction
{
    public interface IDynamicExtractor<in TEvent, TValue> where TValue : struct
    {
        IDynamicExtractorSession<TEvent, TValue> MakeSession();
    }
}