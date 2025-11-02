namespace Sonosthesia.Extractor
{
    public interface IDynamicExtractor<in TEvent, TValue> where TValue : struct
    {
        IDynamicExtractorSession<TEvent, TValue> MakeSession();
    }

    public static class DynamicExtractorExtensions
    {
        public static IDynamicExtractorSession<TEvent, TValue> SetupSession<TEvent, TValue>(
            this IDynamicExtractor<TEvent, TValue> extractor, TEvent e, out TValue result) where TValue : struct
        {
            IDynamicExtractorSession<TEvent, TValue> session = extractor.MakeSession();
            session.Setup(e, out result);
            return session;
        }   
    }
}