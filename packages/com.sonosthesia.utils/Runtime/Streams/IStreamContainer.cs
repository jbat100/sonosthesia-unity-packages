namespace Sonosthesia.Utils
{
    public interface IStreamContainer<T> where T : struct
    {
        StreamNode<T> StreamNode { get; }
    }
}