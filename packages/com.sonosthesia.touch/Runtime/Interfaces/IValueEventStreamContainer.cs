using Sonosthesia.Utils;

namespace Sonosthesia.Touch
{
    // TODO : split base source actor and affordance into interaction package
    
    /// <summary>
    /// Events which contain a given value type, for observers which want to handle value specifics
    /// not just the interaction event
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    public interface IValueEventStreamContainer<TValue, TEvent> 
        where TValue : struct 
        where TEvent : struct, IValueEvent<TValue>
    {
        StreamNode<TEvent> ValueStreamNode { get; }
    }
}