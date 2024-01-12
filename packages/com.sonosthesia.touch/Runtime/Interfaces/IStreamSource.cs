using Sonosthesia.Utils;

namespace Sonosthesia.Touch
{
    public interface IStreamSource<TEvent> where TEvent : struct
    {
        StreamNode<TEvent> SourceStreamNode { get; }
    }
}