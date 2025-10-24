namespace Sonosthesia.Interaction
{
    public interface IPostProcessing<TValue> where TValue : struct
    {
        TValue PostProcess(TValue TValue);
    }
}