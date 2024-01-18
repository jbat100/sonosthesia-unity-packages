using Sonosthesia.Utils;

namespace Sonosthesia.Touch
{
    public interface IValueStreamSource<TValue, TEvent> 
        where TValue : struct where TEvent : struct, IEventValue<TValue>
    {
        StreamNode<TEvent> ValueStreamNode { get; }
    }
}