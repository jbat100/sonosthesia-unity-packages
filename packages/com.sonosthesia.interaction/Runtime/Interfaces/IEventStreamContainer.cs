using Sonosthesia.Utils;

namespace Sonosthesia.Interaction
{
    /// <summary>
    /// Event stream with no specified value type, allows observers to be value type agnostic
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IEventStreamContainer<TEvent> where TEvent : struct
    {
        StreamNode<TEvent> EventStreamNode { get; }
    }
}