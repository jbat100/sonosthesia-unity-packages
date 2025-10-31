namespace Sonosthesia.Extractor
{
    public interface IStaticExtractor<in TEvent, TValue>
    {
        bool Extract(TEvent e, out TValue value);
    }
}